using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Handles all player states, its transitions and common parameters.
    /// </summary>
    public class PlayerStateMachine : MonoBehaviour
    {
        [field: Header("REFERENCES"), HorizontalLine(2F, EColor.Red)]
        [field: SerializeField, Required] public PlayerHealthHandler HealthHandler { get; private set; }
        [field: SerializeField, Required] public PlayerInput PlayerInput { get; private set; }
        [field: SerializeField, Required] public Rigidbody Body { get; private set; }
        [field: SerializeField, Required] public Animator Animator { get; private set; }
        [field: SerializeField, Required] public SpriteRenderer SpriteRenderer { get; private set; }
        [field: SerializeField, Required] public BoxCollider ContextSpeedBounds { get; private set; }

        [field: Header("STATE REFERENCES"), HorizontalLine(2F, EColor.Orange)]
        [field:SerializeField] public PlayerIdleState Idle { get; private set; }
        [field: SerializeField] public PlayerWalkingState Walking { get; private set; }
        [field: SerializeField] public PlayerStunnedState Stunned { get; private set; }
        [field: SerializeField] public PlayerDeathState Death { get; private set; }
        [field: SerializeField] public PlayerAttackingState Attacking { get; private set; }
        [field: SerializeField] public PlayerDefendingState Defending { get; private set; }


        [field: Header("STATE VARIABLES"), HorizontalLine(2F, EColor.Yellow)]
        [field: SerializeField, ReadOnly] public PlayerState CurrentState { get; private set; }
        [ReadOnly] public PlayerState nextState;
        [ReadOnly] public bool overrideStateTransition;
        [ReadOnly] public bool canBeStunned = true;
        [SerializeField, ReadOnly] bool canInput = true;
        [field: SerializeField, ReadOnly] public Vector2 InputDirection { get; private set; } = Vector2.zero;

        [SerializeField, ReadOnly] Collider[] contextVelocityColliders;
        [field: SerializeField, ReadOnly] public Vector3 ContextVelocityAdditive { get; private set; }
        [field: SerializeField, ReadOnly] public float ContextVelocityMultiplier { get; private set; }

        [SerializeField] LayerMask conveyorLayer;
        [SerializeField] LayerMask speedMultiplierLayer;

        void Awake()
        {
            //Setup all states
            PlayerState[] childStates = GetComponentsInChildren<PlayerState>();
            foreach(var child in childStates)
            {
                child.Setup(this);
            }
            //Start on the idle state
            CurrentState = Idle;
            nextState = Idle;
            CurrentState.Enter();
        }

        void Start()
        {
            FollowGroup.Instance.AddTarget(transform);
            HealthHandler.OnDamageTaken += OnDamageTaken;
            HealthHandler.OnDeath += OnDeath;
            HealthHandler.OnRevive += OnRevive;

            //Dont allow Inputs device switch on multiplayer
            if(GameManager.Instance.PlayerCharacterList.Count <= 1)
            {
                PlayerInput.neverAutoSwitchControlSchemes = false;
            }

        }
        void Update()
        {
            SwitchState();
            CurrentState.Do();
        }

        void FixedUpdate()
        {
            GetContextSpeed();
            CurrentState.FixedDo();
        }
        /// <summary>
        /// Calculates the combined force of all context speed triggers in contact with the player.
        /// </summary>
        void GetContextSpeed()
        {
            Vector3 tempContextSpeed = Vector3.zero;
            float tempSpeedMultiplier = 1f;
            Physics.OverlapBoxNonAlloc(transform.position, ContextSpeedBounds.size/2, contextVelocityColliders);

            //Additive context velocity
            foreach(Collider collider in contextVelocityColliders)
            {
                if (!conveyorLayer.ContainsLayer(collider.gameObject.layer)) continue;
                if (!collider.transform.TryGetComponent<ConveyorBelt>(out ConveyorBelt belt)) continue;                   
                tempContextSpeed += belt.ContextSpeed;          
            }
            ContextVelocityAdditive = tempContextSpeed;

            //Multiplicative context velocity
            foreach (Collider collider in contextVelocityColliders)
            {
                if (!speedMultiplierLayer.ContainsLayer(collider.gameObject.layer)) continue;
                if (!collider.transform.TryGetComponent<ConveyorBelt>(out ConveyorBelt belt)) continue;
                tempSpeedMultiplier *= 1;
            }
            ContextVelocityMultiplier = tempSpeedMultiplier;

        }

        void SwitchState()
        {
            if (overrideStateTransition)
            {
                CurrentState.Exit();
                CurrentState = nextState;
                nextState = null;
                CurrentState.Enter();
                overrideStateTransition = false;
                return;
            }

            if (CurrentState.CanTransition)
            {
                if (nextState == null) nextState = ChoseState();
                if (nextState == null) return;

                CurrentState.Exit();
                CurrentState = nextState;
                nextState = null;
                CurrentState.Enter();
            }
        }

        PlayerState ChoseState()
        {
            if (InputDirection.magnitude > 0)
            {
                if (CurrentState != Walking) return Walking;
                else return null;
            }

            if (CurrentState != Idle) return Idle;

            return null;
        }
        void OnDamageTaken(float damage, Vector3 _knockback, float _duration)
        {
            if (!canBeStunned) return;

            Stun(_knockback, _duration);
        }

        void OnDeath(Vector2 _knockback, float _duration)
        {
            Death.knockBackIntensity = _knockback;
            nextState = Death;
            overrideStateTransition = true;
        }

        void OnRevive()
        {
            transform.parent.gameObject.SetActive(true);
            FollowGroup.Instance.AddTarget(transform);
            HealthHandler.UpdateHealthUI();
            nextState = Idle;
            overrideStateTransition = true;
        }

        #region Animation Events
        public void PlayFootstepSound()
        {
            Walking.PlayFootstepSound();
        }

        #endregion

        public void Stun(Vector3 _knockback, float _duration)
        {
            overrideStateTransition = true;
            Stunned.knockBackIntensity = _knockback;
            Stunned.duration = _duration;
            nextState = Stunned;
        }

        public void ValidateAttack(InputAction.CallbackContext context)
        {
            if (!canInput) return;

            if (Time.deltaTime <= 0) return;

            if (CurrentState == Stunned || CurrentState == Death) return;

            if (!context.performed) return;

            Attacking.ValidateAttack(context);
        }

        public void UseItem(InputAction.CallbackContext context)
        {
            if (!canInput) return;

            if (Time.deltaTime <= 0) return;

            if (!context.performed) return;

            if (CurrentState == Stunned || CurrentState == Death) return;

            GameObject storedItem = GameManager.Instance.PlayerCharacterList[PlayerInput.playerIndex].StoredItem;

            if (storedItem == null) return;

            if (!storedItem.TryGetComponent<ItemDrop>(out ItemDrop item)) return;

            item.Use(PlayerInput.playerIndex);
        }

        public void PauseGame(InputAction.CallbackContext context)
        {
            if (!canInput) return;

            if (!context.performed) return;

            if (SceneManager.GetActiveScene().buildIndex != 1) return;

            GameManager.Instance.PauseGame();
        }

        public void PauseGame(PlayerInput input)
        {
            if (!canInput) return;

            if (input != PlayerInput) return;

            if (SceneManager.GetActiveScene().buildIndex != 1) return;

            GameManager.Instance.PauseGame();
        }

        public void GetInputDirection(InputAction.CallbackContext context)
        {
            if (!canInput)
            {
                InputDirection = Vector2.zero;
                return;
            }

            if (Time.deltaTime <= 0) return;

            InputDirection = context.ReadValue<Vector2>();
        }
        
        public void Defend(InputAction.CallbackContext context)
        {
            if (!canInput) return;

            if (Time.deltaTime <= 0) return;

            if (CurrentState == Stunned || CurrentState == Death) return;

            if (!context.performed) return;

            Attacking.queuedAttackState = null;
            nextState = Defending;
        }

        public void ToggleInput(bool active)
        {
            canInput = active;
            InputDirection = Vector2.zero;
        }
    }
}

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
        [Header("REFERENCES"), HorizontalLine(2F, EColor.Red)]
        public PlayerInput playerInput;
        public Rigidbody body;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public BoxCollider footCollider;

        [field: Header("STATE REFERENCES"), HorizontalLine(2F, EColor.Orange)]
        [field:SerializeField] public PlayerIdleState Idle { get; private set; }
        [field: SerializeField] public PlayerWalkingState Walking { get; private set; }
        [field: SerializeField] public PlayerStunnedState Stunned { get; private set; }
        [field: SerializeField] public PlayerDeathState Death { get; private set; }
        [field: SerializeField] public PlayerAttackingState Attacking { get; private set; }


        [field: Header("STATE VARIABLES"), HorizontalLine(2F, EColor.Yellow)]
        [field: SerializeField, ReadOnly] public PlayerState CurrentState { get; private set; }
        [ReadOnly] public PlayerState nextState;
        [ReadOnly] public bool overrideStateCompletion;

        [field: SerializeField ,ReadOnly] public Vector2 InputDirection { get; private set; } = Vector2.zero;

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
            //Dont allow Inputs device switch on multiplayer
            if(GameManager.Instance.PlayerCharacterList.Count <= 1)
                playerInput.neverAutoSwitchControlSchemes = false;
        }
        void Update()
        {
            SelectState();
            CurrentState.Do();
        }

        void FixedUpdate()
        {
            GetContextSpeed();
            CurrentState.FixedDo();
            transform.parent.position = transform.parent.position.ToXZZ();
        }
        /// <summary>
        /// Calculates the combined force of all context speed triggers in contact with the player.
        /// </summary>
        void GetContextSpeed()
        {
            Vector3 tempContextSpeed = Vector3.zero;
            float tempSpeedMultiplier = 1f;
            Collider[] cntxSpdColliders = Physics.OverlapBox(transform.position, footCollider.size/2);

            //Additive context velocity
            foreach(Collider collider in cntxSpdColliders)
            {
                if (!conveyorLayer.ContainsLayer(collider.gameObject.layer)) continue;
                if (!collider.transform.TryGetComponent<ConveyorBelt>(out ConveyorBelt belt)) continue;                   
                tempContextSpeed += belt.ContextSpeed;          
            }
            ContextVelocityAdditive = tempContextSpeed;

            //Multiplicative context velocity
            foreach (Collider collider in cntxSpdColliders)
            {
                if (!speedMultiplierLayer.ContainsLayer(collider.gameObject.layer)) continue;
                if (!collider.transform.TryGetComponent<ConveyorBelt>(out ConveyorBelt belt)) continue;
                tempSpeedMultiplier *= 1;
            }
            ContextVelocityMultiplier = tempSpeedMultiplier;

        }

        void SelectState()
        {
            if (CurrentState.IsComplete)
            {
                CurrentState.Exit();
                CurrentState = nextState;
                CurrentState.Enter();
            }
            else if (overrideStateCompletion)
            {
                CurrentState.Exit();
                CurrentState = nextState;
                CurrentState.Enter();
                overrideStateCompletion = false;
            }
        }
        public void TakeDamage(bool isDead, Vector2 _knockback, float _duration)
        {
            overrideStateCompletion = true;
            if (isDead == false)
            {
                Stunned.knockBackIntensity = _knockback;
                Stunned.duration = _duration;
                nextState = Stunned;
            }
            else
            {
                Death.knockBackIntensity = _knockback;
                nextState = Death;
            }
        }

        #region Animation Events
        public void PlayFootstepSound()
        {
            Walking.PlayFootstepSound();
        }

        #endregion
        public void ValidateAttack(InputAction.CallbackContext context)
        {
            if (Time.deltaTime <= 0) return;

            if (!context.performed) return;

            if (CurrentState == Stunned || CurrentState == Death) return;

            Attacking.ValidateAttack(context);

        }

        public void UseItem(InputAction.CallbackContext context)
        {
            if (Time.deltaTime <= 0) return;

            if (!context.performed) return;

            if (CurrentState == Stunned || CurrentState == Death) return;

            GameObject storedItem = GameManager.Instance.PlayerCharacterList[playerInput.playerIndex].StoredItem;

            if (storedItem == null) return;

            if (!storedItem.TryGetComponent<ItemDrop>(out ItemDrop item)) return;

            item.Use(playerInput.playerIndex);
        }

        public void PauseGame(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (SceneManager.GetActiveScene().buildIndex != 1) return;

            GameManager.Instance.PauseGame();
        }

        public void PauseGame(PlayerInput input)
        {
            if (input != playerInput) return;

            if (SceneManager.GetActiveScene().buildIndex != 1) return;

            GameManager.Instance.PauseGame();
        }

        public void GetInputDirection(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (Time.deltaTime <= 0) return;

            InputDirection = context.ReadValue<Vector2>();
        }        
    }
}

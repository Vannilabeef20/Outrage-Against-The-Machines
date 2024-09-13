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

        [field: SerializeField, ReadOnly] public Vector3 ContextVelocity { get; private set; }

        [SerializeField] LayerMask conveyorLayer;

        [SerializeField] private TextMeshProUGUI playerNumberLabel;
        [SerializeField, ReadOnly] Color playerColor;
        Color player1Color = Color.cyan;
        Color player2Color = Color.green;
        Color player3Color = Color.yellow;


        [Header("Debug"), HorizontalLine(2F, EColor.Green)]
        [SerializeField] TextMeshProUGUI playerStateLabel;

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
            playerStateLabel.text = CurrentState.Name;
        }

        void Start()
        {
            //Dont allow Inputs device switch on multiplayer
            if(GameManager.Instance.PlayerCharacterList.Count <= 1)
                playerInput.neverAutoSwitchControlSchemes = false;

            //Assign player color & number
            switch (playerInput.playerIndex)
            {
                case 0:
                    playerColor = player1Color;
                    break;
                case 1:
                    playerColor = player2Color;
                    break;
                case 2:
                    playerColor = player3Color;
                    break;
            }
            playerNumberLabel.text = $"P{playerInput.playerIndex + 1}";
            playerNumberLabel.color = playerColor;
        }
        void Update()
        {
            SelectState();
            CurrentState.Do();
            RunDebug();
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
            Collider[] cntxSpdColliders = Physics.OverlapBox(transform.position, footCollider.size/2);
            foreach(Collider collider in cntxSpdColliders)
            {
                if (!conveyorLayer.ContainsLayer(collider.gameObject.layer)) continue;
                if (!collider.transform.TryGetComponent<ConveyorBelt>(out ConveyorBelt belt)) continue;                   
                tempContextSpeed += belt.ContextSpeed;          
            }
            ContextVelocity = tempContextSpeed;
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
            if(Time.deltaTime <= 0)
            {
                return;
            }
            if(!context.performed)
            {
                return;
            }
            if (CurrentState != Stunned && CurrentState != Death)
            {
                Attacking.ValidateAttack(context);
            }
        }

        public void PauseGame(InputAction.CallbackContext context)
        {
            if(SceneManager.GetActiveScene().buildIndex == 0)
            {
                return;
            }
            if (!context.performed)
            {
                return;
            }
            GameManager.Instance.PauseGame();
        }

        public void GetInputDirection(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }
            if (Time.deltaTime <= 0)
            {
                return;
            }
            InputDirection = context.ReadValue<Vector2>();
        }

        void RunDebug()
        {
            if (CustomLogger.IsDebugModeEnabled)
            {
                if (playerStateLabel.gameObject.activeInHierarchy == false)
                {
                    playerStateLabel.gameObject.SetActive(true);
                }
                playerStateLabel.transform.rotation = Quaternion.identity;
                playerStateLabel.text = CurrentState.Name;
            }
            else
            {
                if (playerStateLabel.gameObject.activeInHierarchy == true)
                {
                    playerStateLabel.gameObject.SetActive(false);
                }
            }
        }
    }
}

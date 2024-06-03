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
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("REFERENCES"), HorizontalLine]
        public PlayerInput playerInput;
        public Rigidbody body;
        public AudioSource audioSource;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public BoxCollider footCollider;

        [field: Header("STATE REFERENCES"), HorizontalLine(2F, EColor.Red)]
        [field:SerializeField] public PlayerIdleState Idle { get; private set; }
        [field: SerializeField] public PlayerWalkingState Walking { get; private set; }
        [field: SerializeField] public PlayerStunnedState Stunned { get; private set; }
        [field: SerializeField] public PlayerDeathState Death { get; private set; }
        [field: SerializeField] public PlayerAttackingState Attacking { get; private set; }


        [field: Header("STATE VARIABLES"), HorizontalLine(2F, EColor.Orange)]
        [field: SerializeField, ReadOnly] public PlayerState CurrentState { get; private set; }
        [ReadOnly] public PlayerState nextState;
        [ReadOnly] public bool overrideStateCompletion;

        [field: SerializeField ,ReadOnly] public Vector2 InputDirection { get; private set; } = Vector2.zero;

        [field: SerializeField, ReadOnly] public Vector3 ContextVelocity { get; private set; }

        [SerializeField] private LayerMask conveyorLayer;

        [Header("Debug"), HorizontalLine(2F, EColor.Yellow)]
        [SerializeField] private TextMeshProUGUI playerStateLabel;

        private void Awake()
        {
            spriteRenderer.material.SetFloat("PlayerID", (float)playerInput.user.id);
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

        private void Start()
        {
            if(GameManager.Instance.PlayerCharacterList.Count <= 1)
            {
                playerInput.neverAutoSwitchControlSchemes = false;
            }
        }
        private void Update()
        {
            
            SelectState();
            CurrentState.Do();
            if (DebugManager.instance.isDebugModeEnabled)
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

        private void FixedUpdate()
        {
            GetContextSpeed();
            CurrentState.FixedDo();
            transform.parent.transform.position = new Vector3(transform.position.x, transform.position.z, transform.position.z);
        }

        private void GetContextSpeed()
        {
            Vector3 tempContextSpeed = Vector3.zero;
            Collider[] colliders = Physics.OverlapBox(transform.position, footCollider.size/2);
            foreach(Collider collider in colliders)
            {
                if (conveyorLayer.ContainsLayer(collider.gameObject.layer))
                {
                    if (collider.transform.TryGetComponent<ConveyorBelt>(out ConveyorBelt belt))
                    {
                        tempContextSpeed += belt.ContextSpeed;
                    }
                }
            }
            ContextVelocity = tempContextSpeed;
        }
        private void SelectState()
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

    }
}

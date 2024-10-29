using UnityEngine;
using NaughtyAttributes;
using TMPro;
#if UNITY_EDITOR
#endif

namespace Game
{
    [SelectionBase]
    public class BossStateMachine : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Rigidbody body;
        public Animator animator;
        public BoxCollider hurtBox;
        public BoxCollider attackhitbox;
        public BoxCollider collisionBox;
        [field: SerializeField] public GameObject Parent { get; private set; }
        

        #region State References
        [Header("STATE REFERENCES"), HorizontalLine(2F, EColor.Red)]

        public MK1InterceptState intercept;
        public MK1DamageState damage;
        public MK1AttackingState attack;
        public MK1DeathState death;

        #endregion

        #region State Variables
        [Header("STATE VARIABLES"), HorizontalLine(2F, EColor.Orange)]
        [ReadOnly] public bool overrideStateCompletion;
        [SerializeField, ReadOnly] private BossState currentState;
        [ReadOnly] public BossState nextState;
        [Space]
        [SerializeField] private LayerMask conveyorLayer;
        [field: SerializeField, ReadOnly] public Vector3 ContextVelocity;
        #endregion

        #region Aligment Check
        [Header("TARGETING"), HorizontalLine(2f, EColor.Green)]
        [SerializeReference, SubclassSelector] BaseTargeting targetingBehaviour;
        [Space]
        [SerializeField, Min(0f)] float targetingRepeatInterval;
        [field: SerializeField, ReadOnly] public Transform Target { get; private set; }
        [SerializeField, ReadOnly] float targetingTimer;
        [field: SerializeField, ReadOnly] public float Distance { get; private set; }
       
        public bool IsInsidePlayzone => LevelManager.Instance.IsInsidePlayzone(body.position);
        #endregion

        private void OnEnable()
        {
            hurtBox.enabled = true;
        }
        private void OnDisable()
        {
            Spawner.Instance.enemiesAlive.Remove(Parent);
        }
        private void Start()
        {
            BossState[] states = GetComponentsInChildren<BossState>();
            foreach (BossState state in states)
            {
                state.Setup(this);
            }
            currentState = intercept;
            nextState = intercept;
            currentState.Enter();
            if (currentState != null)
            {
                stateLabelTmpro.text = currentState.Name;
            }
        }

        private void Update()
        {
            targetingTimer += Time.deltaTime;
            //Refresh Target
            if (targetingTimer > targetingRepeatInterval)
            {
                Target = targetingBehaviour.GetTarget(body.position);
                targetingTimer = 0;
            }

            if (Target != null) Distance = Vector3.Distance(transform.position, Target.position);
            else Distance = -1;

            ChangeState();
            currentState.Do();

            if (CustomLogger.IsDebugModeEnabled)
            {
                if (stateLabelTmpro.gameObject.activeInHierarchy == false)
                {
                    stateLabelTmpro.gameObject.SetActive(true);
                }
                stateLabelTmpro.transform.rotation = Quaternion.identity;
            }
            else
            {
                if (stateLabelTmpro.gameObject.activeInHierarchy == true)
                {
                    stateLabelTmpro.gameObject.SetActive(false);
                }
            }
        }

        private void FixedUpdate()
        {
            GetContextSpeed();
            currentState.FixedDo();
            body.position = body.position.ToXYY();
        }

        private void ChangeState()
        {
            if (currentState.IsComplete)
            {
                currentState.Exit();
                currentState = nextState;
                currentState.Enter();
                if (currentState != null)
                {
                    stateLabelTmpro.text = currentState.Name;
                }
            }
            else if (overrideStateCompletion)
            {
                currentState.Exit();
                currentState = nextState;
                currentState.Enter();
                overrideStateCompletion = false;
                if (currentState != null)
                {
                    stateLabelTmpro.text = currentState.Name;
                }
            }
        }

        public void TakeDamage(Vector3 _damageDealerPos, float _stunDuration, float _knockbackStrenght)
        {
            attackhitbox.enabled = false;
            damage.stunDuration = _stunDuration;
            damage.damageDealerPos = _damageDealerPos;
            damage.knockbackStrenght = _knockbackStrenght;
            nextState = damage;
            overrideStateCompletion = true;
        }

        private void GetContextSpeed()
        {
            Vector3 tempContextSpeed = Vector3.zero;
            Collider[] colliders = Physics.OverlapBox(transform.position, collisionBox.size / 2);
            foreach (Collider collider in colliders)
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

        #region Debug
        [Header("DEBUG"), HorizontalLine(2F, EColor.Green)]
        [SerializeField] private TextMeshProUGUI stateLabelTmpro;
        #endregion
    }
}

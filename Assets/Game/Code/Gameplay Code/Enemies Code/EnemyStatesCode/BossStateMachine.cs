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
        public BoxCollider mk1Attackhitbox;
        public BoxCollider mk2Attackhitbox;
        public BoxCollider collisionBox;
        [field: SerializeField] public GameObject Parent { get; private set; }
        

        #region State References
        [Header("STATE REFERENCES"), HorizontalLine(2F, EColor.Red)]

        public MK1InterceptState mk1Intercept;
        public MK1DamageState mk1Damage;
        public MK1AttackingState mk1Attack;
        public MK1DeathState mk1Death;

        public MK2InterceptState mk2Intercept;
        public MK2DamageState mk2Damage;
        public MK2AttackingState mk2Attack;
        public MK2DeathState mk2Death;

        #endregion

        #region State Variables
        [Header("STATE VARIABLES"), HorizontalLine(2F, EColor.Orange)]

        [ReadOnly] public bool phase2;
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
            currentState = mk1Intercept;
            nextState = mk1Intercept;
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
            if(!phase2)
            {
                mk1Attackhitbox.enabled = false;
                mk1Damage.stunDuration = _stunDuration;
                mk1Damage.damageDealerPos = _damageDealerPos;
                mk1Damage.knockbackStrenght = _knockbackStrenght;
                nextState = mk1Damage;
            }
            else
            {
                mk2Attackhitbox.enabled = false;
                mk2Damage.stunDuration = _stunDuration;
                mk2Damage.damageDealerPos = _damageDealerPos;
                mk2Damage.knockbackStrenght = _knockbackStrenght;
                nextState = mk2Damage;
            }
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

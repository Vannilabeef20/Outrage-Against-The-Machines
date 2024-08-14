using UnityEngine;
using NaughtyAttributes;
using TMPro;
#if UNITY_EDITOR
#endif

namespace Game
{
    [SelectionBase]
    public class EnemyStateMachine : MonoBehaviour
    {
        public Spawner spawner;
        public SpriteRenderer spriteRenderer;
        public Rigidbody body;
        public Animator animator;
        public BoxCollider hurtBox;
        public BoxCollider attackhitbox;
        public BoxCollider collisionBox;

        #region State References
        [Header("STATE REFERENCES"), HorizontalLine(2F, EColor.Red)]
        public EnemyInterceptState intercept;
        public EnemyDamageState damage;
        public EnemyAttackingState attack;
        public EnemyDeathState death;
        #endregion

        #region State Variables
        [Header("STATE VARIABLES"), HorizontalLine(2F, EColor.Orange)]
        [SerializeField] private EnemyState currentState;
        public EnemyState nextState;
        [ReadOnly] public bool overrideStateCompletion;
        [field: SerializeField, ReadOnly] public Vector3 ContextVelocity;
        [SerializeField] private LayerMask conveyorLayer;
        #endregion

        #region Aligment Check
        [field: Header("ALIGMENT CHECK")]
        [field: SerializeField, ReadOnly] public bool IsAligned { get; private set; }
        [field: SerializeField, ReadOnly] public float Distance { get; private set; }
        [SerializeField] private LayerMask boxCastLayerMask;
        [field: SerializeField] public Vector3 BoxCastOffset { get; private set; }
        [SerializeField] private Vector3 boxCastDimensions;
        [SerializeField] private float boxCastLenght;

        [field: SerializeField, ReadOnly] public bool IsInsidePlayZone { get; private set; }

        #endregion

        #region Debug

        [Header("DEBUG"), HorizontalLine(2F, EColor.Green)]
        [SerializeField, Expandable] DebugSO debugSO;
        [SerializeField] private TextMeshProUGUI stateLabelTmpro;
#if UNITY_EDITOR
        [SerializeField] private Color boxCastDefaultColor;
        [SerializeField] private Color boxCastAlignedColor;
#endif
        #endregion

        private void Awake()
        {
            try
            {
                if (transform.parent.TryGetComponent<Spawner>(out Spawner spawn))
                {
                    spawner = spawn;
                }
            }
            catch
            {

            }
        }

        private void OnEnable()
        {
            if(spawner != null)
            {
                spawner.enemiesAlive.Add(gameObject);
            }
            hurtBox.enabled = true;
        }
        private void OnDisable()
        {
            if (spawner != null)
            {
                spawner.enemiesAlive.Remove(gameObject);
            }
        }
        private void Start()
        {
            EnemyState[] states = GetComponentsInChildren<EnemyState>();
            foreach (EnemyState state in states)
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
            IsInsidePlayZone = LevelManager.Instance.IsInsidePlayZone(body.position);
            IsAligned = Physics.BoxCast(transform.position + new Vector3(transform.right.x * BoxCastOffset.x,
                BoxCastOffset.y, BoxCastOffset.z), boxCastDimensions, transform.right,
                out RaycastHit info, Quaternion.identity, boxCastLenght, boxCastLayerMask);
            if (IsAligned)
            {
                Distance = info.distance;
            }
            else
            {
                Distance = -1;
            }
            ChangeState();
            currentState.Do();

            if (debugSO.IsDebugModeEnabled)
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


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (IsAligned)
            {
                Helper.DrawBoxCastBox(transform.position + new Vector3(transform.right.x * BoxCastOffset.x,
                    BoxCastOffset.y, BoxCastOffset.z), boxCastDimensions, Quaternion.identity,
                        transform.right, boxCastLenght, boxCastAlignedColor);
            }
            else
            {
                Helper.DrawBoxCastBox(transform.position + new Vector3(transform.right.x * BoxCastOffset.x,
                    BoxCastOffset.y, BoxCastOffset.z), boxCastDimensions, Quaternion.identity,
                        transform.right, boxCastLenght, boxCastDefaultColor);
            }
        }
#endif

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
    }
}

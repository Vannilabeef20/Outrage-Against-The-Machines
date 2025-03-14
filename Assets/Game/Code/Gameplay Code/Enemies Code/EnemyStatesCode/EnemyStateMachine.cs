using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

namespace Game
{
    [SelectionBase]
    public class EnemyStateMachine : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Rigidbody body;
        public Animator animator;
        public BoxCollider hurtBox;
        public BoxCollider attackhitbox;
        public BoxCollider collisionBox;
        [field: SerializeField] public GameObject Parent { get; private set; }
        [Space]
        public bool IsFieldEnemy;
        [SerializeField, Range(0,1)] float screenPercentage = 0.8f;
        [Space]
        [ReadOnly] public bool IsOnScreen;


        #region State References
        [Header("STATE REFERENCES"), HorizontalLine(2F, EColor.Red)]
        public EnemyWanderState wander;
        public EnemyChageModeState change;
        public EnemyInterceptState intercept;
        public EnemyDamageState damage;
        public EnemyAttackingState attack;
        public EnemyDeathState death;

        #endregion

        #region State Variables
        [Header("STATE VARIABLES"), HorizontalLine(2F, EColor.Orange)]
        [ReadOnly] public bool overrideStateCompletion;
        [SerializeField, ReadOnly] private EnemyState currentState;
        [ReadOnly] public EnemyState nextState;
        [Space]
        [SerializeField] private LayerMask conveyorLayer;
        [SerializeField, ReadOnly] public Vector3 ContextVelocity;
        #endregion

        #region Aligment Check
        [field: Header("ALIGMENT CHECK")]
        [field: SerializeField, ReadOnly] public bool IsAligned { get; private set; }
        [field: SerializeField, ReadOnly] public float Distance { get; private set; }
        [SerializeField] private LayerMask boxCastLayerMask;
        [field: SerializeField] public Vector3 BoxCastOffset { get; private set; }
        [SerializeField] private Vector3 boxCastDimensions;
        [SerializeField] private float boxCastLenght;

        #endregion

        #region Debug

        [Header("DEBUG (THIS WILL BE STRIPPED ON BUILD)"), HorizontalLine(2F, EColor.Green)]
        [SerializeField] private TextMeshProUGUI stateLabelTmpro;
#if UNITY_EDITOR
        [SerializeField] private Color boxCastDefaultColor;
        [SerializeField] private Color boxCastAlignedColor;
#endif
        #endregion

        private void OnEnable()
        {
            if (IsFieldEnemy == false)
                Spawner.Instance.enemiesAlive.Add(Parent);

            hurtBox.enabled = true;
        }
        private void OnDisable()
        {
            Spawner.Instance.enemiesAlive.Remove(Parent);
        }
        private void Start()
        {
            EnemyState[] states = GetComponentsInChildren<EnemyState>();
            foreach (EnemyState state in states)
            {
                state.Setup(this);
            }

            if(IsFieldEnemy)
            {
                currentState = wander;
                nextState = wander;
            }
            else
            {
                currentState = intercept;
                nextState = intercept;
            }

            currentState.Enter();
            if (currentState != null)
            {
                stateLabelTmpro.text = currentState.Name;
            }
        }

        private void Update()
        {
            IsOnScreen = GameManager.Instance.OnScreenPercent(screenPercentage, spriteRenderer.GetSpriteCorners());

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

            Vector3[] corners = spriteRenderer.GetSpriteCorners();
            Vector3 nextPoint;
            Color lineColor;
            if (IsOnScreen) { lineColor = Color.cyan; }
            else lineColor = Color.blue;

            for (int i = 0; i < corners.Length; i++)
            {
                if(i == corners.Length - 1) { nextPoint = corners[0];}
                else { nextPoint = corners[i + 1]; }

                Debug.DrawLine(corners[i], nextPoint, lineColor);
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

        void ChangeState()
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

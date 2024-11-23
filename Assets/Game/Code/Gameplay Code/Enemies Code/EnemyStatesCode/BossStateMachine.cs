using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;
using FMODUnity;
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
        [SerializeField] BoolEvent toggleLevelMusicEvent;
        [SerializeField] StudioEventEmitter bossMusicEmitter;
        [SerializeField] Image healhbarImage;
        [SerializeField] Sprite healthBarPhase2Sprite;

        [field: SerializeField] public GameObject Parent { get; private set; }

        [SerializeField] GameObject uiObject;
        [SerializeField] int encounterIndex;

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


        [SerializeField, ReadOnly] bool phase2;
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

        private void OnDisable()
        {
            toggleLevelMusicEvent.Raise(this, true);
            Spawner.Instance.enemiesAlive.Remove(Parent);
        }

        private void Update()
        {
            if (currentState == null) return;

            targetingTimer += Time.deltaTime;
            //Refresh Target
            if (targetingTimer > targetingRepeatInterval)
            {
                Target = targetingBehaviour.GetTarget(body.position);
                targetingTimer = 0;
            }

            if (Target != null) Distance = Vector3.Distance(transform.position, Target.position);
            else Distance = -1;

            currentState.Do();
            ChangeState();
        }

        private void FixedUpdate()
        {
            if (currentState == null) return;

            GetContextSpeed();
            currentState.FixedDo();
            body.position = body.position.ToXZZ();
        }

        public void Init(int index)
        {
            if (index != encounterIndex) return;
            if (phase2) return;

            uiObject.SetActive(true);
            toggleLevelMusicEvent.Raise(this, false);
            bossMusicEmitter.Play();
            hurtBox.enabled = true;
            Spawner.Instance.enemiesAlive.Add(Parent);
            Target = targetingBehaviour.GetTarget(body.position);
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

        public void Stun(Vector3 _damageDealerPos, float _stunDuration, float _knockbackStrenght)
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
        public void Kill()
        {
            if (!phase2)
            {
                mk1Attackhitbox.enabled = false;
                nextState = mk1Death;
            }
            else
            {
                mk2Attackhitbox.enabled = false;
                nextState = mk2Death;
            }
            overrideStateCompletion = true;
        }

        public void SetPhase2()
        {
            phase2 = true;
            healhbarImage.sprite = healthBarPhase2Sprite;
            healhbarImage.SetNativeSize();
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

        public void Flip()
        {
            if (Target == null) return;
            //Flip
            if (Target.transform.position.x + 0.1f < transform.position.x)
            {
                Parent.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if (Target.transform.position.x - 0.1f > transform.position.x)
            {
                Parent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

        #region Debug
        [Header("DEBUG"), HorizontalLine(2F, EColor.Green)]
        [SerializeField] private TextMeshProUGUI stateLabelTmpro;
        #endregion
    }
}

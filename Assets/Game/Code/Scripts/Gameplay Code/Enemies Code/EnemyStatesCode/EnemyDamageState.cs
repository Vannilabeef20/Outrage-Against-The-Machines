using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using Cinemachine;

namespace Game
{
    public class EnemyDamageState : EnemyState
    {
        public override string Name { get => "Damage"; }
        [field: Header("DAMAGE STATE"), HorizontalLine(2f, EColor.Yellow)]
        [field: SerializeField] public EnemyHealthHandler HealthHandler { get; private set; }
        [SerializeField] private AudioClip damageTakenSound;
        [SerializeField] private ParticleSystem damageParticles;

        [SerializeField] private LayerMask enviriomentLayerMask;
        #region Enemy Hit Flash
        [SerializeField, ReadOnly] private Color startingColor;
        [SerializeField] private Color hitFlashColor;
        [SerializeField] private float hitFlashLenght;
        [SerializeField, ReadOnly] private float hitFlashTimer;
        #endregion
        #region Enemy Knockback
        [Header("KNOCKBACK"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] private AnimationCurve knockbackCurve;
        [ReadOnly] public Vector3 damageDealerPos;
        [ReadOnly] public Vector3 initialPos;
        [ReadOnly] public float stunDuration;
        [ReadOnly] public float knockbackStrenght;
        #endregion

        #region Enemy Poise Params
        [Header("ENEMY POISE"), HorizontalLine(2f, EColor.Blue)]
        [Tooltip("This enemy's poise points")]
        [SerializeField, Range(0f,1f)] private float[] poisePoints;


        [Tooltip("This enemy's max poise points")]
        [SerializeField, Min(0f)] private float maxPoise;

        [Tooltip("This enemy's current poise points")]
        [SerializeField, ReadOnly] private float currentPoise;

        [Tooltip("This enemy's current poise ratio 0-1")]
        [SerializeField, ReadOnly] private float poiseRatio;

        [Tooltip("This enemy's Poise regen per second")]
        [SerializeField, Min(0f)] private float poiseRegenValue;

        [Tooltip("This enemy's poise regen delay in seconds")]
        [SerializeField, Min(0f)] private float poiseRegenDelay;

        #endregion

        #region Enemy Damage Camera Shake
        [Header("CAMERA SHAKE"), HorizontalLine(2f, EColor.Pink)]

        [SerializeField] private CinemachineImpulseSource impulseSource;
        #endregion


        public override void Do()
        {
            progress = UpTime.Map(0, stunDuration);
            hitFlashTimer += Time.deltaTime;
            if(hitFlashTimer >= hitFlashLenght)
            {
                if(stateMachine.spriteRenderer.color == startingColor)
                {
                    stateMachine.spriteRenderer.color = hitFlashColor;
                }
                else
                {
                    stateMachine.spriteRenderer.color = startingColor;
                }
                hitFlashTimer = 0f;
            }
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = knockbackCurve.Evaluate(progress) *
                knockbackStrenght * Mathf.Sign(initialPos.x - damageDealerPos.x) * Vector3.right;
            if(Physics.Raycast(transform.position, stateMachine.body.velocity.normalized, 0.4f, enviriomentLayerMask))
            {
                stateMachine.body.velocity = Vector3.zero;
            }
        }

        public override void Enter()
        {
            IsComplete = false;
            startingColor = stateMachine.spriteRenderer.color;
            startTime = Time.time;
            hitFlashTimer = 0f;
            initialPos = transform.position;
            ExecuteFeedbacks();
        }

        public override void Exit()
        {
            stateMachine.spriteRenderer.color = startingColor;
        }

        protected override void ValidateState()
        {
            if (UpTime < stunDuration)
            {
                return;
            }
            if(HealthHandler.CurrentHealthPoints <= 0) //temp, should complete deathState 
            {
                stateMachine.nextState = stateMachine.death;
                IsComplete = true;
            }
            else
            {
                stateMachine.nextState = stateMachine.intercept;
                IsComplete = true;
            }
            
        }

        private void ExecuteFeedbacks()
        {
            damageParticles.Play();
            stateMachine.audioSource.PlayOneShot(damageTakenSound);
            impulseSource.GenerateImpulse();
        }

    }
}

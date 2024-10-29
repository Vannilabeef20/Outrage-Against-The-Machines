using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using Cinemachine;
using FMODUnity;

namespace Game
{
    public class MK2DamageState : BossState
    {
        public override string Name { get => "AtkDamage"; }
        [field: Header("DAMAGE STATE"), HorizontalLine(2f, EColor.Yellow)]
        [field: SerializeField] public EnemyHealthHandler HealthHandler { get; private set; }
        [SerializeField] StudioEventEmitter damageTakenEmitter;
        [SerializeField] ParticleSystem damageParticles;
        [SerializeField] ScrapDropper scrapDropper;

        [SerializeField] LayerMask enviriomentLayerMask;
        [Space]
        [ReadOnly] public float stunDuration;
        #region Enemy Hit Flash
        [Header("HIT FLASH"), HorizontalLine(2F, EColor.Green)]
        [SerializeField, ReadOnly] Color startingColor;
        [SerializeField] Color hitFlashColor;
        [SerializeField] float hitFlashLenght;
        [Space]
        [SerializeField, ReadOnly] float hitFlashTimer;
        #endregion
        #region Enemy Knockback
        [Header("KNOCKBACK"), HorizontalLine(2f, EColor.Blue)]
        [SerializeField] AnimationCurve knockbackCurve;
        [Space]
        [ReadOnly] public float knockbackStrenght;
        [ReadOnly] public Vector3 damageDealerPos;
        [ReadOnly] public Vector3 initialPos;
        #endregion

        #region Enemy Damage Camera Shake
        [Header("CAMERA SHAKE"), HorizontalLine(2f, EColor.Indigo)]

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
                knockbackStrenght * Mathf.Sign(initialPos.x - damageDealerPos.x) * Vector3.right + stateMachine.ContextVelocity;
            if(Physics.Raycast(transform.position, stateMachine.body.velocity.normalized, 0.4f, enviriomentLayerMask))
            {
                stateMachine.body.velocity = Vector3.zero + stateMachine.ContextVelocity;
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
            scrapDropper.SpawnRandomScrap();
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
            damageTakenEmitter.Play();
            impulseSource.GenerateImpulse();
        }

        
        #region Enemy Poise Params
        //[Header("ENEMY POISE"), HorizontalLine(2f, EColor.Blue)]
        //[Tooltip("This enemy's poise points")]
        //[SerializeField, Range(0f,1f)] private float[] poisePoints;


        //[Tooltip("This enemy's max poise points")]
        //[SerializeField, Min(0f)] private float maxPoise;

        //[Tooltip("This enemy's current poise points")]
        //[SerializeField, ReadOnly] private float currentPoise;

        //[Tooltip("This enemy's current poise ratio 0-1")]
        //[SerializeField, ReadOnly] private float poiseRatio;

        //[Tooltip("This enemy's Poise regen per second")]
        //[SerializeField, Min(0f)] private float poiseRegenValue;

        //[Tooltip("This enemy's poise regen delay in seconds")]
        //[SerializeField, Min(0f)] private float poiseRegenDelay;

        #endregion
    }
}

using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using Cinemachine;
using FMODUnity;

namespace Game
{
    public class MK2DamageState : BossState
    {
        public override string Name { get => "MK2 Damage"; }
        [field: Header("DAMAGE STATE"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField] StudioEventEmitter damageTakenEmitter;
        [SerializeField] ParticleSystem damageParticles;

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
            if (hitFlashTimer >= hitFlashLenght)
            {
                if (stateMachine.spriteRenderer.color == startingColor)
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
            stateMachine.body.linearVelocity = knockbackCurve.Evaluate(progress) *
                knockbackStrenght * Mathf.Sign(initialPos.x - damageDealerPos.x) * Vector3.right + stateMachine.ContextVelocity;
            if (Physics.Raycast(transform.position, stateMachine.body.linearVelocity.normalized, 0.4f, enviriomentLayerMask))
            {
                stateMachine.body.linearVelocity = Vector3.zero + stateMachine.ContextVelocity;
            }
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            startingColor = Color.white;
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
            if (UpTime < stunDuration) return;

            stateMachine.nextState = stateMachine.mk2Intercept;
            IsComplete = true;

        }

        private void ExecuteFeedbacks()
        {
            damageParticles.Play();
            damageTakenEmitter.Play();
            impulseSource.GenerateImpulse();
        }
    }
}

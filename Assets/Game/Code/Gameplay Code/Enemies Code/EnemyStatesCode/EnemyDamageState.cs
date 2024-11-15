using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using Cinemachine;
using FMODUnity;

namespace Game
{
    public class EnemyDamageState : EnemyState
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


        public override void Do()
        {
            progress = UpTime.Map(0, stunDuration);
            hitFlashTimer += Time.deltaTime;
            if(hitFlashTimer >= hitFlashLenght)
            {
                if(SpriteColor == startingColor)
                {
                    SpriteColor = hitFlashColor;
                }
                else
                {
                    SpriteColor = startingColor;
                }
                hitFlashTimer = 0f;
            }
            MachineAnimator.Play(StateAnimation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            Velocity = knockbackCurve.Evaluate(progress) *
                knockbackStrenght * Mathf.Sign(initialPos.x - damageDealerPos.x) * Vector3.right + stateMachine.ContextVelocity;
            if(Physics.Raycast(transform.position, Velocity.normalized, 0.4f, enviriomentLayerMask))
            {
                Velocity = Vector3.zero + stateMachine.ContextVelocity;
            }
        }

        public override void Enter()
        {
            IsComplete = false;
            startingColor = SpriteColor;
            startTime = Time.time;
            hitFlashTimer = 0f;
            initialPos = transform.position;
            damageParticles.Play();
            damageTakenEmitter.Play();
            scrapDropper.SpawnRandomScrap();
        }

        public override void Exit()
        {
            SpriteColor = startingColor;
        }

        protected override void ValidateState()
        {
            if (UpTime < stunDuration)
            {
                return;
            }
            if(HealthHandler.CurrentHealthPoints <= 0) //temp, should complete deathState 
            {
                NextState = stateMachine.death;
                IsComplete = true;
            }
            else
            {
                NextState = stateMachine.intercept;
                IsComplete = true;
            }         
        }

    }
}

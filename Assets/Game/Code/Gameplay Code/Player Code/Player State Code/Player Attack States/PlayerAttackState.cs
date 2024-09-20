using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public class PlayerAttackState : PlayerState
    {
        [field: Header("PLAYER ATTACK"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField, ReadOnly] PlayerAttackingState AttackMachine;
        [field: SerializeField, Expandable] public PlayerAttackSO PlayerAttack { get; private set; }

        [Header("PROJECTILE"), HorizontalLine(2F, EColor.Green)]

        [SerializeField] public bool hasProjectile;
        [SerializeField, ShowIf("hasProjectile")] public GameObject projectile;
        [SerializeField, ShowIf("hasProjectile")] Transform projectileSpawnTransform;

        [SerializeField] AnimationFrameEvent[] FrameEvents;

        public float TimeLeft  => PlayerAttack.Duration - UpTime;

        StudioEventEmitter EventEmitter => AttackMachine.attackEmitter;

        private void Awake()
        {
            foreach (var frameEvent in FrameEvents)
            {
                frameEvent.Setup(PlayerAttack.Animation, PlayerAttack.Duration);
            }
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            AttackMachine.attackList.Add(PlayerAttack);
            foreach(var frameEvent in FrameEvents)
            {
                frameEvent.Reset();
            }
        }

        public override void Exit()
        {
            AttackMachine.DisableAttackHitboxes();
        }

        public override void Do()
        {
            progress = UpTime.Map(0, PlayerAttack.Duration);
            stateMachine.animator.Play(PlayerAttack.Animation.name, 0, progress);
            foreach (var frameEvent in FrameEvents)
            {
                frameEvent.Update(UpTime);
            }
            ValidateState();
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = (PlayerAttack.VelocityCurve.Evaluate(progress) *
                PlayerAttack.MaxVelocity * transform.right) + stateMachine.ContextVelocity;
        }

        protected override void ValidateState()
        {
            if (UpTime >= PlayerAttack.Duration)
            {
                IsComplete = true;
            }
        }

        public override void Setup(PlayerStateMachine playerStateMachine)
        {
            base.Setup(playerStateMachine);
            AttackMachine = playerStateMachine.Attacking;
        }

        public void PlayPitchedAttackSound(int attackIndex)
        {
            EventEmitter.Play();
            EventEmitter.EventInstance.setPitch(PlayerAttack.AudioPitches[attackIndex]);
            EventEmitter.EventInstance.setParameterByNameWithLabel(
            PlayerAttack.EventParameter, PlayerAttack.EventLabel);
        }

        public void SpawnProjectile()
        {
            if (!hasProjectile) return;

            GameObject projectileInstance = Instantiate(projectile, projectileSpawnTransform.position, Quaternion.identity);

            Destroy(projectileInstance, 5f);
        }

    }
}

using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class PlayerAttackState : PlayerState
    {
        [field: Header("PLAYER ATTACK INHERITED"), HorizontalLine(2f, EColor.Yellow)]

        [field: SerializeField, ReadOnly] protected PlayerAttackingState AttackMachine { get; private set; }

        [field: SerializeField, Expandable] public PlayerAttackSO PlayerAttack { get; private set; }

        [field: SerializeField] public AnimationFrameEvent[] FrameEvents { get; private set; }

        public float TimeLeft  => PlayerAttack.Duration - UpTime;

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
            stateMachine.audioSource.pitch = 1;
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
            AttackMachine.attackAudioSource.pitch = PlayerAttack.AudioPitches[attackIndex];
            AttackMachine.attackAudioSource.PlayOneShot(PlayerAttack.Sound);
        }

    }
}

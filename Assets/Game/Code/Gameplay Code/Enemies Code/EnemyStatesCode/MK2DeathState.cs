using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class MK2DeathState : BossState
    {
        public override string Name { get => "MK2 Death"; }

        [field: Header("DEATH STATE"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField] float duration;
        [SerializeField] AnimationFrameEvent[] frameEvents;

        private void Start()
        {
            foreach (var frame in frameEvents)
            {
                frame.Setup(StateAnimation, duration);
            }
        }
        public override void Do()
        {
            progress = UpTime.Map(0, duration);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
            foreach (var frame in frameEvents)
            {
                frame.Update(UpTime);
            }
            ValidateState();
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = stateMachine.ContextVelocity;
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            stateMachine.mk1Attackhitbox.enabled = false;
            stateMachine.hurtBox.enabled = false;
            stateMachine.animator.Play(StateAnimation.name);
        }

        public override void Exit() { }

        protected override void ValidateState()
        {
            if (UpTime <= duration) return;

            IsComplete = true;
            stateMachine.nextState = stateMachine.mk2Intercept;
            foreach (var frame in frameEvents)
            {
                frame.Reset();
            }
            Destroy(stateMachine.Parent);
        }
    }
}
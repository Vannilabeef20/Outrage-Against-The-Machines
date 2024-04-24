using UnityEngine;
using NaughtyAttributes;
using Cinemachine;

namespace Game
{
    public class PlayerDeathState : PlayerState
    {
        public override string Name { get => "Death"; }

        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float Delay { get; private set; }

        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AnimationCurve knockBackCurve;
        [ReadOnly] public Vector2 knockBackIntensity;

        [Tooltip("Determines how much the lower part of the gamepad will shake.")]
        [SerializeField, Range(0f, 1f)] private float hitGamepadShakeLowFrequency;
        [Tooltip("Determines how much the upper part of the gamepad will shake.")]
        [SerializeField, Range(0f, 1f)] private float hitGamepadShakeHighFrequency;
        [Tooltip("Determines for how long the gamepad will shake.")]
        [SerializeField] private float hitGamepadShakeDuration;

        public override void Do()
        {
            progress = UpTime.Map(0, Duration, 0, 1, true);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = knockBackCurve.Evaluate(progress) * knockBackIntensity;
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            impulseSource.GenerateImpulse();
            foreach(var device in stateMachine.playerInput.devices)
            {
                GameManager.Instance.Rumble(device, hitGamepadShakeLowFrequency,
                    hitGamepadShakeHighFrequency, hitGamepadShakeDuration);
            }
        }

        public override void Exit()
        {
            GameManager.Instance.TakeAddLife(-1);
            if(GameManager.Instance.CurrentLifeAmount <= 0)
            {
                GameManager.Instance.LoadScene(0);
            }
        }

        protected override void ValidateState()
        {
            if (UpTime < Duration + Delay)
            {
                return;
            }
            stateMachine.nextState = stateMachine.Idle;
            IsComplete = true;
        }
    }
}

using UnityEngine;
using NaughtyAttributes;
using Cinemachine;

namespace Game
{
    public class PlayerDeathState : PlayerState
    {
        [field: Header("STATE"), HorizontalLine(2f, EColor.Yellow)]
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] public bool Despawning { get; private set; }
        public override string Name { get => "Death"; }


        [SerializeField] private PlayerDeathParamsEvent playerDeathParamsEvent;

        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AnimationCurve knockBackCurve;
        [ReadOnly] public Vector2 knockBackIntensity;

        [Header("Gamepad Shake"), HorizontalLine]
        [SerializeField, Range(0f, 1f)] private float hitGamepadShakeLowFrequency;
        [SerializeField, Range(0f, 1f)] private float hitGamepadShakeHighFrequency;
        [SerializeField] private float hitGamepadShakeDuration;

        public override void Do()
        {
            progress = UpTime.Map(0, Duration);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = (Vector3)(knockBackCurve.Evaluate(progress) * knockBackIntensity) + stateMachine.ContextVelocity;
        }

        public override void Enter()
        {
            Despawning = false;
            IsComplete = false;
            startTime = Time.time;
            stateMachine.audioSource.PlayOneShot(deathSound);
            impulseSource.GenerateImpulse();
            foreach(var device in stateMachine.playerInput.devices)
            {
                GameManager.Instance.Rumble(device, hitGamepadShakeLowFrequency,
                    hitGamepadShakeHighFrequency, hitGamepadShakeDuration);
            }
        }

        public override void Exit()
        {

        }

        protected override void ValidateState()
        {
            if (UpTime < Duration + Delay)
            {
                return;
            }
            if(Despawning)
            {
                return;
            }
            Despawning = true;
            IsComplete = true;
            GameManager.Instance.TakeAddLife(-1);
            stateMachine.Attacking.UpdateSpecialBar(-stateMachine.Attacking.specialChargeAmount);
            stateMachine.nextState = stateMachine.Idle;
            if (GameManager.Instance.CurrentLifeAmount <= 0)
            {
                playerDeathParamsEvent.Raise(this, new PlayerDeathParams(stateMachine.playerInput.playerIndex, true));
                stateMachine.transform.parent.gameObject.SetActive(false);
            }            
        }
    }
}

using UnityEngine;
using NaughtyAttributes;
using Cinemachine;
using FMODUnity;

namespace Game
{
    public class PlayerDeathState : PlayerState
    {
        [field: Header("STATE"), HorizontalLine(2f, EColor.Yellow)]
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] public bool Despawning { get; private set; }
        public override string Name { get => "Death"; }

        [SerializeField] StudioEventEmitter soundEmitter;
        [SerializeField] PlayerDeathParamsEvent playerDeathEvent;

        [SerializeField] CinemachineImpulseSource impulseSource;
        [SerializeField] AnimationCurve knockBackCurve;
        [ReadOnly] public Vector2 knockBackIntensity;

        [Header("Gamepad Shake"), HorizontalLine]
        [SerializeField, Range(0f, 1f)] float hitGamepadShakeLowFrequency;
        [SerializeField, Range(0f, 1f)] float hitGamepadShakeHighFrequency;
        [SerializeField] float hitGamepadShakeDuration;

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
            soundEmitter.Play();
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
            stateMachine.Attacking.SetSpecialCharges(0f);
            stateMachine.nextState = stateMachine.Idle;
            if (GameManager.Instance.CurrentLifeAmount <= 0)
            {
                playerDeathEvent.Raise(this, new PlayerDeathParams(stateMachine.playerInput.playerIndex, true));
                stateMachine.transform.parent.gameObject.SetActive(false);
            }            
        }
    }
}

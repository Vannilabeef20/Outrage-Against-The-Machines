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
        Vector3 KnockbackVelocity => knockBackCurve.Evaluate(progress) * knockBackIntensity;
        int PlayerIndex => stateMachine.playerInput.playerIndex;
        string RumbleId => $"P{PlayerIndex + 1} {Name}";

        [Header("Gamepad Shake"), HorizontalLine]
        [SerializeField] RumbleData deathRumble;

        public override void Do()
        {
            progress = UpTime.Map(0, Duration);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = stateMachine.ContextVelocityMultiplier *
                (KnockbackVelocity + stateMachine.ContextVelocityAdditive);
        }

        public override void Enter()
        {
            Despawning = false;
            IsComplete = false;
            startTime = Time.time;
            soundEmitter.Play();
            impulseSource.GenerateImpulse();
            RumbleManager.Instance.CreateRumble(RumbleId, deathRumble, PlayerIndex);
        }

        public override void Exit()
        {

        }

        protected override void ValidateState()
        {
            if (UpTime < Duration + Delay) return;

            if (Despawning) return;

            Despawning = true;
            stateMachine.Attacking.SetSpecialCharges(0f);

            if (GameManager.Instance.CurrentLifeAmount == 0)
            {
                playerDeathEvent.Raise(this, new PlayerDeathParams(stateMachine.playerInput.playerIndex, true));
                stateMachine.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                stateMachine.nextState = stateMachine.Idle;
                IsComplete = true;
                GameManager.Instance.TakeAddLife(-1);
            }
        }
    }
}

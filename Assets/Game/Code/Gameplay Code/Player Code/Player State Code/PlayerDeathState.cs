using UnityEngine;
using NaughtyAttributes;
using Cinemachine;
using FMODUnity;

namespace Game
{
    public class PlayerDeathState : PlayerState
    {

        [Header("STATE"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField] IntEvent itemEvent;
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] public bool Dying { get; private set; }
        public override string Name { get => "Death"; }

        [SerializeField] StudioEventEmitter soundEmitter;

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
            Dying = false;
            IsComplete = false;
            startTime = Time.time;

            soundEmitter.Play();
            impulseSource.GenerateImpulse();
            RumbleManager.Instance.CreateRumble(RumbleId, deathRumble, PlayerIndex);
            GameManager.Instance.PlayerCharacterList[PlayerIndex].isDead = true;
            GameManager.Instance.PlayerCharacterList[PlayerIndex].RemoveItem();
            itemEvent.Raise(this, PlayerIndex);
        }

        public override void Exit() 
        {
            GameManager.Instance.PlayerCharacterList[PlayerIndex].isDead = false;
            stateMachine.Attacking.SetSpecialCharges(0f);
        }


        protected override void ValidateState()
        {
            if (UpTime < Duration + Delay) return;

            if (Dying) return;
            Dying = true;

            if(GameManager.Instance.CurrentLifeAmount > 0)
            {
                GameManager.Instance.TakeAddLife(-1);
                stateMachine.healthHandler.Revive();
            }
            else
            {
                FollowGroup.Instance.RemoveTarget(stateMachine.transform);
                stateMachine.transform.parent.gameObject.SetActive(false);

                int count = 0;
                foreach(var player in GameManager.Instance.PlayerCharacterList)
                {
                    if (!player.GameObject.activeInHierarchy) count++;
                }

                if(count == GameManager.Instance.PlayerCharacterList.Count)
                {
                    TransitionManager.Instance.LoadScene(0);
                }
            }
        }
    }
}

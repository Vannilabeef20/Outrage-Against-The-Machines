using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public class PlayerAttackState : PlayerState
    {
        [Header("PLAYER ATTACK"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField, ReadOnly] PlayerAttackingState AttackMachine;
        public override string Name
        {
            get
            {
                return $"{PlayerAttack.name}";
            }
        }
        [field: SerializeField, Expandable] public PlayerAttackSO PlayerAttack { get; private set; }

        [Header("PROJECTILE"), HorizontalLine(2F, EColor.Green)]

        [SerializeField] public bool hasProjectile;
        [SerializeField, ShowIf("hasProjectile")] public GameObject projectile;
        [SerializeField, ShowIf("hasProjectile")] Transform projectileSpawnTransform;

        [Header("FRAME EVENTS"), HorizontalLine(2F, EColor.Blue)]
        [SerializeField] AnimationFrameEvent[] FrameEvents;

        Rigidbody Body => stateMachine.body;
        Vector3 AttackVelocity => PlayerAttack.VelocityCurve.
            Evaluate(progress) * PlayerAttack.MaxVelocity * transform.right;
        public float TimeLeft  => PlayerAttack.Duration - UpTime;
        int PlayerIndex => stateMachine.playerInput.playerIndex;

        string RumbleId => $"P{PlayerIndex + 1} {Name}";

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
            
            if(stateMachine.nextState == stateMachine.Stunned ||
                stateMachine.CurrentState == stateMachine.Stunned)
            RumbleManager.Instance.CancelRumble(RumbleId);
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
            stateMachine.body.velocity = stateMachine.ContextVelocityMultiplier * 
                (AttackVelocity + stateMachine.ContextVelocityAdditive);
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

        public void PlayAttackRumble()
        {
            RumbleManager.Instance.CreateRumble(RumbleId, PlayerAttack.AtkRumbleData, PlayerIndex);
        }

        public void SpawnProjectile()
        {
            if (!hasProjectile) return;

            GameObject projectileInstance = Instantiate(projectile, projectileSpawnTransform.position, Quaternion.identity);

            Destroy(projectileInstance, 5f);
        }

    }
}

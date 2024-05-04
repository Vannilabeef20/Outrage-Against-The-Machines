using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class PlayerAttackState : PlayerState
    {
        [field: Header("PLAYER ATTACK INHERITED"), HorizontalLine(2f, EColor.Yellow)]

        [field: SerializeField, ReadOnly] protected PlayerAttackingState AttackMachine { get; private set; }

        [field: SerializeField, ReadOnly] public PlayerAttackSO playerAttack { get; private set; }

        public float TimeLeft  => playerAttack.Duration - UpTime;


        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            AttackMachine.attackList.Add(playerAttack);
        }

        public override void Exit()
        {
            AttackMachine.DisableAttackHitboxes();
        }

        public override void Do()
        {
            progress = UpTime.Map(0, playerAttack.Duration);
            stateMachine.animator.Play(playerAttack.Animation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = playerAttack.VelocityCurve.Evaluate(progress) *
                playerAttack.MaxVelocity * transform.right;
        }

        protected override void ValidateState()
        {
            if (UpTime >= playerAttack.Duration)
            {
                IsComplete = true;
            }
        }

        public void Initialize(PlayerStateMachine playerMachine, PlayerAttackingState attackMachine, PlayerAttackSO playerAttack)
        {
            this.stateMachine = playerMachine;
            this.AttackMachine = attackMachine;
            this.playerAttack = playerAttack;
            this.StateAnimation = playerAttack.Animation;
        }

    }
}

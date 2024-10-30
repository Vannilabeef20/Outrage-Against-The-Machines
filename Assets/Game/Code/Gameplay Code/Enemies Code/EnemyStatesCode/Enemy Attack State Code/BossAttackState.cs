using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using FMODUnity;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class BossAttackState : BossState
    {
        [Header("ATTACK STATE"), HorizontalLine(2F, EColor.Yellow)]
        [SerializeField] MK2AttackingState AttackMachine;
        [field: SerializeField] public EnemyAttack Attack { get; private set; }

        [field: SerializeField] public EAttackDirection EDirection { get; private set; }


        [SerializeField] UnityEvent OnExitEvent;


        public override void Setup(BossStateMachine bossStateMachine)
        {
            base.Setup(bossStateMachine);
            AttackMachine = stateMachine.mk2Attack;
            Attack.SetupFrameEvents(StateAnimation);
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            foreach (var frameEvent in Attack.FrameEvents)
            {
                frameEvent.Reset();
            }
        }

        public override void Exit()
        {
            OnExitEvent.Invoke();
            IsComplete = false;
        }
        public override void Do()
        {
            foreach (var frameEvent in Attack.FrameEvents)
            {
                frameEvent.Update(UpTime);
            }
            ValidateState();
        }

        public override void FixedDo() { }

        protected override void ValidateState()
        {
            if (UpTime >= Attack.Config.Duration)
            {
                IsComplete = true;
            }
        }
    }

    public enum EAttackDirection
    {
        Front,
        Omni,
    }
}
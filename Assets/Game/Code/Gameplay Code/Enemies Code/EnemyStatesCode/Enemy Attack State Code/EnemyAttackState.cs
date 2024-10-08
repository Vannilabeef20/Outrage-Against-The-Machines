using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using FMODUnity;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class EnemyAttackState : EnemyState
    {
        [Header("ATTACK STATE"), HorizontalLine(2F, EColor.Yellow)]
        [SerializeField] EnemyAttackingState AttackMachine;

        [SerializeField] UnityEvent OnExitEvent;
        [field: SerializeField] public EnemyAttack Attack { get; private set; }


        public override void Setup(EnemyStateMachine _enemyStateMachine)
        {
            base.Setup(_enemyStateMachine);
            AttackMachine = stateMachine.attack;
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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = AttackMachine.RangeLineColor;
            Gizmos.DrawLine(stateMachine.transform.position + stateMachine.BoxCastOffset,
                stateMachine.transform.position + stateMachine.BoxCastOffset +
                (transform.right * Attack.Config.TriggerRange));

            Vector3 attackPoint = transform.position + stateMachine.BoxCastOffset + (transform.right * Attack.Config.TriggerRange);
            Handles.Label(attackPoint + AttackMachine.RangeLabelOffest, $"\n^\n|\n|\n|\n{Attack.Config.Name}\nrange", AttackMachine.attackLabelStyle);
            Gizmos.DrawLine(attackPoint + AttackMachine.RangeLineHeight, attackPoint - AttackMachine.RangeLineHeight);
        }
#endif
    }
}
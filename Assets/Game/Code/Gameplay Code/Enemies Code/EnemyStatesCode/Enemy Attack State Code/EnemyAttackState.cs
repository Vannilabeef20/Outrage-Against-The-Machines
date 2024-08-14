using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class EnemyAttackState : EnemyState
    {
        [SerializeField] EnemyAttackingState AttackMachine;
        [field: SerializeField] public EnemyAttack attack { get; private set; }

        public override void Setup(EnemyStateMachine _enemyStateMachine)
        {
            base.Setup(_enemyStateMachine);
            AttackMachine = stateMachine.attack;
            attack.SetupFrameEvents(StateAnimation);
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            foreach (var frameEvent in attack.FrameEvents)
            {
                frameEvent.Reset();
            }
        }

        public override void Exit()
        {
            IsComplete = false;
        }
        public override void Do()
        {
            foreach (var frameEvent in attack.FrameEvents)
            {
                frameEvent.Update(UpTime);
            }
            ValidateState();
        }

        public override void FixedDo()
        {

        }

        protected override void ValidateState()
        {
            if (UpTime >= attack.Duration)
            {
                IsComplete = true;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (stateMachine == null)
            {
                return;
            }
            Gizmos.color = AttackMachine.RangeLineColor;
            Gizmos.DrawLine(stateMachine.transform.position + stateMachine.BoxCastOffset,
                stateMachine.transform.position + stateMachine.BoxCastOffset +
                (transform.right * attack.TriggerRange));

            Vector3 attackPoint = transform.position + stateMachine.BoxCastOffset + (transform.right * attack.TriggerRange);
            Handles.Label(attackPoint + AttackMachine.RangeLabelOffest, $" <---{attack.Name} range");
            Gizmos.DrawLine(attackPoint + AttackMachine.RangeLineHeight, attackPoint - AttackMachine.RangeLineHeight);
        }
#endif
    }
}
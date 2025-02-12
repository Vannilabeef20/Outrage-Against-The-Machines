using System;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;

namespace Game
{
    public class EnemyAttackingState : EnemyState
    {
        public override string Name { get => CurrentAttack.Config.Name; }
        [SerializeField] private LayerMask playerMask;
        [field: SerializeField] public EnemyAttackState[] Attacks { get; private set; }

        [field: SerializeField, ReadOnly] public EnemyAttackState CurrentAttackState { get; private set; }
        EnemyAttack CurrentAttack => CurrentAttackState.Attack;

        [SerializeField, ReadOnly] private float attackUptime;
#if UNITY_EDITOR
        [field: Header("DEBUG")]

        public GUIStyle attackLabelStyle;
        [field: SerializeField] public Color RangeLineColor { get; private set; }
        [field: SerializeField] public Vector3 RangeLineHeight { get; private set; }
        [field: SerializeField] public Vector3 RangeLabelOffest { get; private set; }
#endif
        [Button("Sort Attacks")]
        void SortAttacks()
        {
            Attacks = Attacks.OrderBy(attacks => attacks.Attack.Config.TriggerRange).ToArray();
        }


        private void Start()
        {
            SortAttacks();
        }

        public override void Do()
        {           
            attackUptime += Time.deltaTime;
            foreach(AnimationFrameEvent timedEvent in CurrentAttack.FrameEvents)
            {
                timedEvent.Update(attackUptime);
            }
            progress = UpTime.Map(0, CurrentAttack.Config.Duration);
            MachineAnimator.Play(CurrentAttack.Config.Animation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            if (CurrentAttackState == null) return;

            Velocity = CurrentAttack.Config.VelocityCurve.Evaluate(progress) *
                CurrentAttack.Config.Velocity * transform.right + stateMachine.ContextVelocity;
        }

        public override void Enter()
        {
            attackUptime = 0;
            foreach (AnimationFrameEvent frameEvent in CurrentAttack.FrameEvents)
            {
                frameEvent.Reset();
            }
            startTime = Time.time;
            IsComplete = false;
        }

        public override void Exit()
        {
            CurrentAttackState.Exit();
            IsComplete = false;
        }

        protected override void ValidateState()
        {
            if(UpTime >= CurrentAttack.Config.Duration)
            {
                NextState = stateMachine.intercept;
                //CurrentAttackState = null;
                IsComplete = true;
            }
        }



        public bool CheckForAndSetAttack()
        {
            if (stateMachine.Distance <= -1) return false;

            int atkIndex = -1;
            bool hasAvailableAttack = false;
            for (int i = 0; i < Attacks.Length; i++)
            {
                if (stateMachine.Distance <= Attacks[i].Attack.Config.TriggerRange)
                {
                    atkIndex = i;
                    hasAvailableAttack = true;
                    break;
                }
            }
            
            if(atkIndex != -1) CurrentAttackState = Attacks[atkIndex];

            return hasAvailableAttack;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!playerMask.ContainsLayer(other.gameObject.layer))
            {
                return;
            }
            if (other.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                damageble.TakeDamage(MachinePosition, CurrentAttack.Config.Damage,
                    CurrentAttack.Config.StunDuration, CurrentAttack.Config.KnockbackStrenght);
            }
        }
    }
}

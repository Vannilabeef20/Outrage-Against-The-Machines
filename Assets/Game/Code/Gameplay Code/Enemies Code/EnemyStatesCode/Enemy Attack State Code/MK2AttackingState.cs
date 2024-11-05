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
    public class MK2AttackingState : BossState
    {
        public override string Name { get => CurrentAttack.Config.Name; }
        [SerializeField] private LayerMask playerMask;
        [field: SerializeField] public BossAttackState[] Attacks { get; private set; }

        [field: SerializeField, ReadOnly] public BossAttackState CurrentAttackState { get; private set; }
        EnemyAttack CurrentAttack => CurrentAttackState.Attack;

        [SerializeField, ReadOnly] float attackUptime;
#if UNITY_EDITOR
        [field: Header("DEBUG")]

        public GUIStyle attackLabelStyle;
        public Color RangeLineColor { get; private set; }
        public Vector3 RangeLineHeight { get; private set; }
        public Vector3 RangeLabelOffest { get; private set; }
#endif

        private void OnTriggerEnter(Collider other)
        {
            if (!playerMask.ContainsLayer(other.gameObject.layer))
            {
                return;
            }
            if (other.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                damageble.TakeDamage(stateMachine.transform.position, CurrentAttack.Config.Damage,
                    CurrentAttack.Config.StunDuration, CurrentAttack.Config.KnockbackStrenght);
            }
        }

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
            progress = UpTime.Map(0, CurrentAttack.Config.Duration);
            CurrentAttackState.Do();
            ValidateState();
        }

        public override void FixedDo()
        {
            if (CurrentAttackState == null) return;
            CurrentAttackState.FixedDo();
        }

        public override void Enter()
        {
            startTime = Time.time;
            stateMachine.Flip();
            IsComplete = false;
            if (CurrentAttackState == null)
                CheckForAndSetAttack();
            CurrentAttackState.Enter();
        }

        public override void Exit()
        {
            CurrentAttackState.Exit();
            CurrentAttackState = null;
            stateMachine.Flip();
        }

        protected override void ValidateState()
        {
            if (CurrentAttackState.IsComplete)
            {
                stateMachine.nextState = stateMachine.mk2Intercept;
                IsComplete = true;
            }
        }

        public bool CheckForAndSetAttack()
        {
            //Has no target, return
            if (stateMachine.Distance <= -1) return false;

            //Check for available attacks
            int atkIndex = -1;
            bool hasAvailableAttack = false;
            for (int i = 0; i < Attacks.Length; i++)
            {
                if (Attacks[i].IsOnCooldown) continue;
                if (stateMachine.Distance > Attacks[i].Attack.Config.TriggerRange) continue;
                if (!Attacks[i].IsAligned()) continue;

                atkIndex = i;
                hasAvailableAttack = true;
                break;
            }

            //Assign most appropriate attack
            if (atkIndex != -1) CurrentAttackState = Attacks[atkIndex];

            return hasAvailableAttack;
        }
    }
}

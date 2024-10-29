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
    public class MK1AttackingState : BossState
    {
        public override string Name { get => CurrentAttack.Config.Name; }
        [SerializeField] private LayerMask playerMask;
        [field: SerializeField] public BossAttackState[] Attacks { get; private set; }

        [field: SerializeField, ReadOnly] public BossAttackState CurrentAttackState { get; private set; }
        EnemyAttack CurrentAttack => CurrentAttackState.Attack;

        [SerializeField, ReadOnly] float attackUptime;

        [SerializeField, ReadOnly] Vector3 attackDirection;
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
            attackUptime += Time.deltaTime;
            foreach(AnimationFrameEvent timedEvent in CurrentAttack.FrameEvents)
            {
                timedEvent.Update(attackUptime);
            }
            progress = UpTime.Map(0, CurrentAttack.Config.Duration);
            stateMachine.animator.Play(CurrentAttack.Config.Animation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            if (CurrentAttackState == null) return;

            switch(CurrentAttackState.EDirection)
            {
                case EAttackDirection.Front:
                    attackDirection = transform.right;
                    break;
                case EAttackDirection.Omni:
                    SetDirection(); 
                    break;
                case EAttackDirection.OmniStatic:
                    //Calculated on frameEvent
                    break;
            }
            stateMachine.body.velocity = CurrentAttack.Config.VelocityCurve.Evaluate(progress) *
CurrentAttack.Config.Velocity * attackDirection + stateMachine.ContextVelocity;
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
            CurrentAttackState = null;

            //Flip
            if (stateMachine.Target.transform.position.x + 0.1f < transform.position.x)
            {
                stateMachine.Parent.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if (stateMachine.Target.transform.position.x - 0.1f > transform.position.x)
            {
                stateMachine.Parent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }


            IsComplete = false;
        }

        protected override void ValidateState()
        {
            if(UpTime >= CurrentAttack.Config.Duration)
            {
                stateMachine.nextState = stateMachine.intercept;
                IsComplete = true;
            }
        }

        public void SetDirection()
        {
            attackDirection = (stateMachine.Target.position -
                stateMachine.transform.position).normalized;
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
                if (stateMachine.Distance <= Attacks[i].Attack.Config.TriggerRange)
                {
                    atkIndex = i;
                    hasAvailableAttack = true;
                    break;
                }
            }
            
            //Assign most appropriate attack
            if(atkIndex != -1) CurrentAttackState = Attacks[atkIndex];

            return hasAvailableAttack;
        }
    }
}

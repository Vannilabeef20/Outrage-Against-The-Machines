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
        public override string Name { get => currentAttack.Name; }
        [SerializeField] private LayerMask playerMask;
        [field: SerializeField] public EnemyAttackState[] Attacks { get; private set; }
        [ReadOnly] public EnemyAttack currentAttack;
        [SerializeField, ReadOnly] private float attackUptime;
#if UNITY_EDITOR
        [field: Header("DEBUG")]

        public GUIStyle attackLabelStyle;
        public Color RangeLineColor { get; private set; }
        public Vector3 RangeLineHeight { get; private set; }
        public Vector3 RangeLabelOffest { get; private set; }
#endif

        public override void Do()
        {           
            attackUptime += Time.deltaTime;
            foreach(AnimationFrameEvent timedEvent in currentAttack.FrameEvents)
            {
                timedEvent.Update(attackUptime);
            }
            progress = UpTime.Map(0, currentAttack.Duration);
            stateMachine.animator.Play(currentAttack.Animation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            if(currentAttack == null)
            {
                return;
            }
            stateMachine.body.velocity = currentAttack.VelocityCurve.Evaluate(progress) *
                currentAttack.Velocity * transform.right + stateMachine.ContextVelocity;
        }

        public override void Enter()
        {
            attackUptime = 0;
            foreach (AnimationFrameEvent frameEvent in currentAttack.FrameEvents)
            {
                frameEvent.Reset();
            }
            startTime = Time.time;
            IsComplete = false;
        }

        public override void Exit()
        {
            IsComplete = false;
        }

        protected override void ValidateState()
        {
            if(UpTime >= currentAttack.Duration)
            {
                stateMachine.nextState = stateMachine.intercept;
                currentAttack = null;
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
                if (Attacks[i].attack.TriggerRange >= stateMachine.Distance)
                {
                    atkIndex = i;
                    hasAvailableAttack = true;
                    break;
                }
            }
            
            if(atkIndex != -1) currentAttack = Attacks[atkIndex].attack;

            return hasAvailableAttack;
        }

        public void SpawnProjectile()
        {
            Instantiate(currentAttack.ProjectilePrefab, currentAttack.ProjectileSpawnTransform.position, transform.rotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!playerMask.ContainsLayer(other.gameObject.layer))
            {
                return;
            }
            if (other.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                damageble.TakeDamage(stateMachine.transform.position, currentAttack.Damage,
                    currentAttack.StunDuration, currentAttack.KnockbackStrenght);
            }
        }
    }
}

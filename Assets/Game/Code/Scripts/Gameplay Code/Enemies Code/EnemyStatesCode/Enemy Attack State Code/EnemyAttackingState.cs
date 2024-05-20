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
        [field: SerializeField] public EnemyAttack[] Attacks { get; private set; }
        [ReadOnly] public EnemyAttack currentAttack;
        [SerializeField, ReadOnly] private float attackUptime;
#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private Color rangeLineColor;
        [SerializeField] private Color rangePointColor;
        [SerializeField] private float rangePointRadius;
        [SerializeField] private Vector3 rangePointLabelOffest;
#endif
        private void Awake()
        {
            foreach(EnemyAttack attack in Attacks)
            {
                attack.SetupFrameEvents(attack.Animation);
            }
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
                currentAttack.Velocity * transform.right;
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



        public EnemyAttack GetAppropriateAttack()
        {          
            if (stateMachine.Distance <= -1)
            {
                return null;
            }
            EnemyAttack atk = null;           
            for (int i = 0; i < Attacks.Length; i++)
            {
                if (Attacks[i].TriggerRange >= stateMachine.Distance)
                {
                    atk = Attacks[i];
                    break;
                }
            }
            return atk;
        }

        public void SpawnProjectile()
        {
            Instantiate(currentAttack.ProjectilePrefab, currentAttack.ProjectileSpawnTransform.position, transform.rotation);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if(stateMachine == null)
            {
                return;
            }
            Debug.DrawLine(stateMachine.transform.position + stateMachine.BoxCastOffset,
                stateMachine.transform.position + stateMachine.BoxCastOffset + (transform.right *
                Attacks[Attacks.Length - 1].TriggerRange), rangeLineColor);
            Gizmos.color = rangePointColor;
            foreach (EnemyAttack attack in Attacks)
            {
                Vector3 attackPoint = transform.position + stateMachine.BoxCastOffset + (transform.right * attack.TriggerRange);
                Handles.Label(attackPoint + rangePointLabelOffest, $" <---{attack.Name} range");
                Gizmos.DrawSphere(attackPoint, rangePointRadius);
            }
        }
        private void OnValidate()
        {
            stateMachine = GetComponentInParent<EnemyStateMachine>();
           Attacks = Attacks.OrderBy(attack => attack.TriggerRange).ToArray();
        }
#endif
    }
}

using System;
using UnityEngine;
using System.Linq;
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
#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private Color rangeLineColor;
        [SerializeField] private Color rangePointColor;
        [SerializeField] private float rangePointRadius;
        [SerializeField] private Vector3 rangePointLabelOffest;
#endif

        public override void Do()
        {
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
            if (stateMachine.Distance < 0)
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
        private void OnTriggerEnter(Collider other)
        {
            if(!playerMask.ContainsLayer(other.gameObject.layer))
            {
                return;
            }
            if (other.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                damageble.TakeDamage(stateMachine.transform.position, currentAttack.Damage,
                    currentAttack.StunDuration, currentAttack.KnockbackStrenght);
            }
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

    [Serializable]
    public class EnemyAttack
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AnimationClip Animation { get; private set; }
        [field: SerializeField] public float TriggerRange { get; private set; }
        [field: SerializeField] public AnimationCurve VelocityCurve { get; private set; }
        [field: SerializeField] public float Velocity { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float StunDuration { get; private set; }
        [field: SerializeField] public float KnockbackStrenght { get; private set; }
    }

}

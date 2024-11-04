using System.Collections;
using System.Collections.Generic;
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
        [field: Header("ATTACK STATE"), HorizontalLine(2F, EColor.Yellow)]

        [field: SerializeField] public EAttackDirection EDirection { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }

        [field: SerializeField, ReadOnly] public bool IsOnCooldown { get; private set; }

        [SerializeField, ReadOnly] Vector3 attackDirection;

        [SerializeField, ReadOnly] bool stopTracking;
        [field: SerializeField] public EnemyAttack Attack { get; private set; }

        [SerializeField] UnityEvent OnExitEvent;

        public override void Setup(BossStateMachine bossStateMachine)
        {
            base.Setup(bossStateMachine);
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
            stopTracking = false;
        }
        public override void Do()
        {
            progress = UpTime.Map(0, Attack.Config.Duration);
            stateMachine.animator.Play(Attack.Config.Animation.name, 0, progress);
            foreach (var frameEvent in Attack.FrameEvents)
            {
                frameEvent.Update(UpTime);
            }
            ValidateState();
        }

        public override void FixedDo() 
        {
            switch (EDirection)
            {
                case EAttackDirection.Front:
                    attackDirection = transform.right;
                    break;
                case EAttackDirection.Omni:
                    if (!stopTracking)
                    {
                        stateMachine.Flip();
                        attackDirection = (stateMachine.Target.position -
                        stateMachine.transform.position).normalized;
                    }
                    break;
            }
            stateMachine.body.velocity = Attack.Config.VelocityCurve.Evaluate(progress) *
            Attack.Config.Velocity * attackDirection + stateMachine.ContextVelocity;
        }

        protected override void ValidateState()
        {
            if (UpTime >= Attack.Config.Duration)
            {
                IsOnCooldown = true;
                StartCoroutine(CooldownRoutine());
                IsComplete = true;
            }
        }

        public void StopTracking()
        {
            stopTracking = true;
        }

        public bool IsAligned()
        {
            switch (EDirection)
            {
                case EAttackDirection.Front: return true;
                case EAttackDirection.Omni: return true;
                default: return false;
            }
        }

        IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(Cooldown);
            IsOnCooldown = false;
        }
    }

    public enum EAttackDirection
    {
        Front,
        Omni,
    }
}
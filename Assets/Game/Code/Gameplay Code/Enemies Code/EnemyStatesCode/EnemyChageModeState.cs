using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyChageModeState : EnemyState
    {
        [SerializeField] float duration;

        public override void Do()
        {
            progress = UpTime.Map(0, duration);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo() { }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            stateMachine.body.velocity = Vector3.zero;
        }

        public override void Exit()
        {
            IsComplete = false;
        }

        protected override void ValidateState()
        {
            if (progress < 1) return;

            stateMachine.nextState = stateMachine.intercept;
            IsComplete = true;
        }
    }
}
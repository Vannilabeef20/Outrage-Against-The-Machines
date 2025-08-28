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
            MachineAnimator.Play(StateAnimation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo() { }

        public override void Enter()
        {
            CanTransition = false;
            startTime = Time.time;
            Velocity = Vector3.zero;
        }

        public override void Exit()
        {
            CanTransition = false;
        }

        protected override void ValidateState()
        {
            if (progress < 1) return;

            NextState = stateMachine.intercept;
            CanTransition = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class PlayerIdleState : PlayerState
    {
        public override string Name { get => "Idle"; }
        [Header("STATE LOCAL"), HorizontalLine(2f, EColor.Green)]
        [SerializeField, Range(0f, 1f)] private float dragIntensity;
        public override void Enter()
        {
            stateMachine.animator.Play(StateAnimation.name);
        }

        public override void Exit()
        {
            IsComplete = false;
        }

        public override void Do()
        {
            ValidateState();
        }
        public override void FixedDo()
        {
            if(stateMachine.ContextVelocity == Vector3.zero)
            {
                stateMachine.body.velocity *= (1 - dragIntensity) * Time.deltaTime;
            }
            else
            {
                stateMachine.body.velocity = stateMachine.ContextVelocity;
            }

        }

        protected override void ValidateState()
        {
            if (stateMachine.InputDirection != Vector2.zero)
            {
                stateMachine.nextState = stateMachine.Walking;
                IsComplete = true;
                return;
            }
        }
    }
}

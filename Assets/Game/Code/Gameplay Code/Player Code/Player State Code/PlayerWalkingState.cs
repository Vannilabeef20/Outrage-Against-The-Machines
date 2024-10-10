using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public class PlayerWalkingState : PlayerState
    {
        public override string Name { get => "Walking"; }
        [Header("STATE LOCAL"), HorizontalLine(2f, EColor.Green)]

        [SerializeField] StudioEventEmitter EmitterFootstep_0;
        [SerializeField] StudioEventEmitter EmitterFootstep_1;
        [SerializeField] Vector2 movementSpeed;
        [SerializeField, ReadOnly] int currentFootstepSoundIndex;
        [SerializeField, ReadOnly] Vector3 velocity;

        public override void Do()
        {
            ValidateState();
            if (stateMachine.InputDirection.x > 0)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); //Flip Right
            }
            else if (stateMachine.InputDirection.x < 0)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0)); //Flip left
            }
            stateMachine.animator.speed = stateMachine.InputDirection.magnitude;
            velocity = new Vector3(stateMachine.InputDirection.x *
                movementSpeed.x, stateMachine.InputDirection.y * movementSpeed.y, stateMachine.InputDirection.y * movementSpeed.y);
            Helper.DrawDirArrow(transform.position, velocity, Color.yellow, Color.green);
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = stateMachine.ContextVelocityMultiplier *
                (velocity + stateMachine.ContextVelocityAdditive);
        }

        public override void Enter()
        {
            stateMachine.animator.Play(StateAnimation.name);
        }

        public override void Exit()
        {
            stateMachine.animator.speed = 1;
            IsComplete = false;
        }

        protected override void ValidateState()
        {
            if (stateMachine.InputDirection == Vector2.zero)
            {
                stateMachine.nextState = stateMachine.Idle;
                IsComplete = true;
                return;
            }
        }

        public void PlayFootstepSound()
        {
            currentFootstepSoundIndex++;
            switch(currentFootstepSoundIndex)
            {
                case 0: 
                    EmitterFootstep_0.Play();
                    break;
                case 1: 
                    EmitterFootstep_1.Play();
                    break;
                default:
                    EmitterFootstep_0.Play();
                    currentFootstepSoundIndex = 0;
                    break;
            }
        }
    }
}

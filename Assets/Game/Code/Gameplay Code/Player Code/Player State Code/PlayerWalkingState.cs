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
                stateMachine.Parent.transform.localScale = Vector3.one; //Flip Right
            }
            else if (stateMachine.InputDirection.x < 0)
            {
                stateMachine.Parent.transform.localScale = new Vector3(-1, 1, 1); //Flip left
            }
            stateMachine.Animator.speed = stateMachine.InputDirection.magnitude;

            velocity.x = stateMachine.InputDirection.x * movementSpeed.x;
            velocity.y = 0;
            velocity.z = stateMachine.InputDirection.y * movementSpeed.y;

            Helper.DrawDirArrow(transform.position, velocity, Color.yellow, Color.green);
        }

        public override void FixedDo()
        {
            stateMachine.Body.linearVelocity = stateMachine.ContextVelocityMultiplier *
                (velocity + stateMachine.ContextVelocityAdditive);
        }

        public override void Enter()
        {
            stateMachine.Animator.Play(StateAnimation.name);
        }

        public override void Exit()
        {
            stateMachine.Animator.speed = 1;
            CanTransition = false;
        }

        protected override void ValidateState()
        {
            CanTransition = true;
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

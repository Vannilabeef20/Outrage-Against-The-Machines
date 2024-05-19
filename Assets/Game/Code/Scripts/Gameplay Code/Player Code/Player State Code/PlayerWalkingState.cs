using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class PlayerWalkingState : PlayerState
    {
        public override string Name { get => "Walking"; }
        [Header("STATE LOCAL"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] private Vector2 movementSpeed;
        [SerializeField] private AudioClip[] footstepSoundClips;
        [SerializeField, ReadOnly] private int currentFootstepSoundIndex;
        [SerializeField, ReadOnly] private Vector3 velocity;
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
            Debug.DrawLine(transform.position, transform.position + velocity, Color.green);
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = velocity;
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
            if(currentFootstepSoundIndex >= footstepSoundClips.Length)
            {
                currentFootstepSoundIndex = 0;
            }
            stateMachine.audioSource.PlayOneShot(footstepSoundClips[currentFootstepSoundIndex]);
        }
    }
}

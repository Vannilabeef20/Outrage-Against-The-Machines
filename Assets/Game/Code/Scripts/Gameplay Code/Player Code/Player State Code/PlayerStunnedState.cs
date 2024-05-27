using System;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

namespace Game
{
    public class PlayerStunnedState : PlayerState
    {
        //Detecting the hit, applying both stun and invulnerability effects
        //and requesting feedbacks wont be handled here, but in the health handler
        public override string Name { get => "Stunned"; }

        [Header("STATE LOCAL"), HorizontalLine(2f, EColor.Yellow)]
        [ReadOnly] public float duration;

        [ReadOnly] public Vector2 knockBackIntensity;
        [Header("PARAMS"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] private AnimationCurve knockBackCurve;
        [SerializeField] private AudioClip damageTakenSound;
        #region CAMERA SHAKE

        //Camera Shake
        [Header("CAMERA SHAKE"), HorizontalLine(2f, EColor.Blue)]

        [SerializeField] private CinemachineImpulseSource impulseSource;

        #endregion
        #region GAMEPAD SHAKE   
        [Header("GAMEPAD SHAKE"), HorizontalLine(2f, EColor.Violet)]

        [Tooltip("Determines how much the lower part of the gamepad will shake.")]
        [SerializeField, Range(0f, 1f)] private float hitGamepadShakeLowFrequency;
        [Tooltip("Determines how much the upper part of the gamepad will shake.")]
        [SerializeField, Range(0f, 1f)] private float hitGamepadShakeHighFrequency;
        [Tooltip("Determines for how long the gamepad will shake.")]
        [SerializeField] private float hitGamepadShakeDuration;

        #endregion

        public override void Do()
        {
            ValidateState();
            progress = UpTime.Map(0, duration);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = (Vector3)(knockBackCurve.Evaluate(progress) * knockBackIntensity) + stateMachine.ContextVelocity;
        }

        public override void Enter()
        {
            IsComplete = false;
            stateMachine.animator.speed = 0;
            startTime = Time.time;
            stateMachine.audioSource.PlayOneShot(damageTakenSound);
            foreach (var device in stateMachine.playerInput.devices)
            {
                GameManager.Instance.Rumble(device, hitGamepadShakeLowFrequency,
                    hitGamepadShakeHighFrequency, hitGamepadShakeDuration);
            }

            impulseSource.GenerateImpulse();
        }

        public override void Exit()
        {
            IsComplete = false;
            stateMachine.animator.speed = 1;
            stateMachine.body.velocity = Vector3.zero;
        }

        protected override void ValidateState()
        {
            if (UpTime < duration)
            {
                return;
            }          
            stateMachine.nextState = stateMachine.Idle;
            IsComplete = true;                
        }

    }
}

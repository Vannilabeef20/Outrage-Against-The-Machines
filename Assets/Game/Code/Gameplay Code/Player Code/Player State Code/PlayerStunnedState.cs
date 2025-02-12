using System;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    /// <summary>
    /// Defines the stun triggers when a player takes a hit.
    /// <para>WARNING!!<br/>
    /// Detecting the hit, applying both stun and invulnerability effects and<br/>
    /// requesting feedbacks wont be handled here, but in the health handler.</para>
    /// </summary>
    public class PlayerStunnedState : PlayerState
    {
        public override string Name { get => "Stunned"; }

        [Header("STATE LOCAL"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] StudioEventEmitter soundEmitter;
        [ReadOnly] public float duration;

        [ReadOnly] public Vector2 knockBackIntensity;
        [Header("PARAMS"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] private AnimationCurve knockBackCurve;
        Vector3 KnockBackVelocity => knockBackCurve.Evaluate(progress) * knockBackIntensity;
        int PlayerIndex => stateMachine.playerInput.playerIndex;
        string RumbleId => $"P{PlayerIndex + 1} {Name}";

        #region CAMERA SHAKE

        //Camera Shake
        [Header("CAMERA SHAKE"), HorizontalLine(2f, EColor.Blue)]

        [SerializeField] private CinemachineImpulseSource impulseSource;

        #endregion
        #region GAMEPAD SHAKE   
        [Header("GAMEPAD SHAKE"), HorizontalLine(2f, EColor.Violet)]

        [SerializeField] RumbleData stunnedRumble;

        #endregion

        public override void Do()
        {
            ValidateState();
            progress = UpTime.Map(0, duration);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = stateMachine.ContextVelocityMultiplier * 
                (KnockBackVelocity + stateMachine.ContextVelocityAdditive);
        }

        public override void Enter()
        {
            IsComplete = false;
            stateMachine.healthHandler.Stun(duration);
            stateMachine.animator.speed = 0;
            startTime = Time.time;
            soundEmitter.Play();
            RumbleManager.Instance.CreateRumble(RumbleId, stunnedRumble, PlayerIndex);
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
	public class PlayerDefendingState : PlayerState
	{
        public override string Name { get => "Stunned"; }

        [Header("STATE LOCAL"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] StudioEventEmitter soundEmitter;
        [SerializeField] StudioEventEmitter hitEmitter;

        [Header("PARAMS"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] float animationDuration;
        [SerializeField] RumbleData rumble;
        [Space]
        [SerializeField] float parryDamageMultiplier;
        [SerializeField, MinMaxSlider(0f, 1f)] Vector2 parryWindow;
        [Space]
        [SerializeField] private AnimationCurve knockBackCurve;
        [SerializeField, Range(0f, 1f)] float knockBackDecay = 0.8f;
        [SerializeField] float maxKnockBackIntensity;
        [ReadOnly] public Vector3 knockBackIntensity;




        public override void Setup(PlayerStateMachine playerStateMachine)
        {
            base.Setup(playerStateMachine);
            playerStateMachine.HealthHandler.OnDamageTaken += OnDamage;
        }

        public override void Do()
        {
            ValidateState();
            progress = UpTime.Map(0, animationDuration);
            stateMachine.Animator.Play(StateAnimation.name, 0, progress);
            if(progress > parryWindow.x && progress < parryWindow.y)
            {
                stateMachine.canBeStunned = false;
                stateMachine.HealthHandler.damageMultiplier = parryDamageMultiplier;
            }
            else
            {
                stateMachine.canBeStunned = true;
                stateMachine.HealthHandler.damageMultiplier = 1f;
            }
        }

        public override void FixedDo()
        {
            knockBackIntensity *= knockBackDecay;

            stateMachine.Body.linearVelocity = stateMachine.ContextVelocityMultiplier *
                (knockBackIntensity + stateMachine.ContextVelocityAdditive);
        }

        public override void Enter()
        {
            CanTransition = false;
            stateMachine.Animator.speed = 0;
            startTime = Time.time;
            knockBackIntensity = Vector3.zero;
            soundEmitter.Play();
            stateMachine.canBeStunned = false;
            RumbleManager.Instance.CreateRumble(this.name, rumble, stateMachine.PlayerInput.playerIndex);
        }

        public override void Exit()
        {
            CanTransition = false;
            stateMachine.HealthHandler.damageMultiplier = 1f;
            stateMachine.canBeStunned = true;
            stateMachine.Animator.speed = 1;
            stateMachine.Body.linearVelocity = Vector3.zero;
            knockBackIntensity = Vector3.zero;
        }

        protected override void ValidateState()
        {
            if (UpTime < animationDuration) return;
            CanTransition = true;
        }

        void OnDamage(float damage, Vector3 knockBack, float stunDuration)
        {
            if (stateMachine.CurrentState != this) return;

            hitEmitter.Play();
            knockBackIntensity = knockBack;
        }
    }
}

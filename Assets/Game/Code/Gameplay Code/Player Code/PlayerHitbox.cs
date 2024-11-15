using UnityEngine;
using NaughtyAttributes;
using FMODUnity;
using Cinemachine;

namespace Game 
{
    /// <summary>
    /// Manages the player
    /// </summary>
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] BoxCollider attackCollider;
        [SerializeField] PlayerStateMachine stateMachine;
        [SerializeField] LayerMask collisionMask;
        [SerializeField] GameObject hitEffect;
        [SerializeField] StudioEventEmitter emitter;
        [SerializeField] CinemachineImpulseSource impulseSource;

        #region Shorthand lambdas
        RumbleData AtkRumble => stateMachine.Attacking.CurrentAttackState.PlayerAttack.AtkRumbleData;

        float AtkDamage => stateMachine.Attacking.CurrentAttackState.PlayerAttack.Damage;
        float AtkStunDuration => stateMachine.Attacking.CurrentAttackState.PlayerAttack.StunDuration;
        float AtkKnockbackStrenght => stateMachine.Attacking.CurrentAttackState.PlayerAttack.KnockbackStrenght;
        bool AtkIsSpecial => stateMachine.Attacking.CurrentAttackState.PlayerAttack.IsSpecial;
        int PlayerIndex => stateMachine.playerInput.playerIndex;
        string RumbleId => $"P{PlayerIndex + 1} Hit";
        #endregion
        void OnTriggerEnter(Collider other)
        {
            if (!collisionMask.ContainsLayer(other.gameObject.layer)) return;

            if (stateMachine.Attacking.CurrentAttackState == null) return;

            PlayHitFeedback(other.bounds.center);

            DealDamage(other);
        }

        void PlayHitFeedback(Vector3 hitObjectPos)
        {
            //Hit Particle
            Instantiate(hitEffect, attackCollider.ClosestPoint(hitObjectPos), Quaternion.identity);
            emitter.Play();

            //Rumble
            RumbleManager.Instance.CreateRumble(RumbleId, AtkRumble, PlayerIndex);

            //ScreenShake
            impulseSource.GenerateImpulse();
        }

        void DealDamage(Collider hitObjectCollider)
        {
            if (hitObjectCollider.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                if (!AtkIsSpecial) stateMachine.Attacking.AddSpecialCharges(AtkDamage);

                damageble.TakeDamage(transform.position, AtkDamage, AtkStunDuration, AtkKnockbackStrenght);
            }
        }
    }
}


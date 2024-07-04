using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

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
        [SerializeField] AudioClip hitSound;
        //[SerializeField, ReadOnly] private List<Collider> prevHitColliders;

        #region Shorthand lambdas
        float AtkRumbleLowFreq => stateMachine.Attacking.CurrentAttackState.PlayerAttack.RumbleLowFrequency;
        float AtkRumbleHighFreq => stateMachine.Attacking.CurrentAttackState.PlayerAttack.RumbleHighFrequency;
        float AtkRumbleDuration => stateMachine.Attacking.CurrentAttackState.PlayerAttack.RumbleDuration;
        float AtkDamage => stateMachine.Attacking.CurrentAttackState.PlayerAttack.Damage;
        float AtkStunDuration => stateMachine.Attacking.CurrentAttackState.PlayerAttack.StunDuration;
        float AtkKnockbackStrenght => stateMachine.Attacking.CurrentAttackState.PlayerAttack.KnockbackStrenght;
        bool AtkIsSpecial => stateMachine.Attacking.CurrentAttackState.PlayerAttack.IsSpecial;
        #endregion
        private void OnTriggerEnter(Collider other)
        {
            if (!collisionMask.ContainsLayer(other.gameObject.layer)) return;

            //if (prevHitColliders.Contains(other)) return;

            if (stateMachine.Attacking.CurrentAttackState == null) return;

            PlayHitFeedback(other.bounds.center);

            DealDamage(other);
        }

        private void PlayHitFeedback(Vector3 hitObjectPos)
        {
            //Hit Particle
            Instantiate(hitEffect, attackCollider.ClosestPoint(hitObjectPos), Quaternion.identity);
            stateMachine.audioSource.PlayOneShot(hitSound);

            //Rumble
            foreach (var device in stateMachine.playerInput.devices)
            {
                GameManager.Instance.Rumble(device, AtkRumbleLowFreq, AtkRumbleHighFreq, AtkRumbleDuration);
            }
        }

        private void DealDamage(Collider hitObjectCollider)
        {
            if (hitObjectCollider.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                //prevHitColliders.Add(hitObjectCollider);

                if (!AtkIsSpecial) stateMachine.Attacking.AddSpecialCharges(AtkDamage);

                damageble.TakeDamage(transform.position, AtkDamage, AtkStunDuration, AtkKnockbackStrenght);
            }
        }

        public void ClearHitColliders()
        {
            //prevHitColliders.Clear();
        }
    }
}


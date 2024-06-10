using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game 
{
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private BoxCollider attackCollider;
        [SerializeField] private PlayerStateMachine stateMachine;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private AudioClip hitSound;

        private void OnTriggerEnter(Collider other)
        {
            if (!layerMask.ContainsLayer(other.gameObject.layer))
            {
                return;
            }
            if (stateMachine.Attacking.CurrentAttackState == null)
            {
                return;
            }
            Instantiate(hitEffect, attackCollider.ClosestPoint(other.bounds.center), Quaternion.identity);
            stateMachine.audioSource.PlayOneShot(hitSound);
            foreach (var device in stateMachine.playerInput.devices)
            {
                GameManager.Instance.Rumble(device, stateMachine.Attacking.CurrentAttackState.PlayerAttack.RumbleLowFrequency,
                    stateMachine.Attacking.CurrentAttackState.PlayerAttack.RumbleHighFrequency,
                    stateMachine.Attacking.CurrentAttackState.PlayerAttack.RumbleDuration);
            }
            if (other.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                float damage = stateMachine.Attacking.CurrentAttackState.PlayerAttack.Damage;
                if(!stateMachine.Attacking.CurrentAttackState.PlayerAttack.IsSpecial)
                {
                    stateMachine.Attacking.AddSpecialCharges(damage);
                }
                damageble.TakeDamage(transform.position, damage, stateMachine.Attacking.CurrentAttackState.PlayerAttack.StunDuration,
                    stateMachine.Attacking.CurrentAttackState.PlayerAttack.KnockbackStrenght);
            }
        }
    }
}


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
            if (other.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                float damage = stateMachine.Attacking.CurrentAttackState.PlayerAttack.Damage;
                stateMachine.Attacking.UpdateSpecialBar(damage);
                damageble.TakeDamage(transform.position, damage, stateMachine.Attacking.CurrentAttackState.PlayerAttack.StunDuration,
                    stateMachine.Attacking.CurrentAttackState.PlayerAttack.KnockbackStrenght);
            }
        }
    }
}


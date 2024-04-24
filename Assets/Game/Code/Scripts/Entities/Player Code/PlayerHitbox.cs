using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private PlayerAttackingState attackingState;
        [SerializeField] private LayerMask layerMask;

        private void OnTriggerEnter(Collider other)
        {
            if (!layerMask.ContainsLayer(other.gameObject.layer))
            {
                return;
            }
            if (attackingState.CurrentAttackState == null)
            {
                return;
            }
            if (other.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                damageble.TakeDamage(transform.position, attackingState.CurrentAttackState.playerAttack.Damage,
                    attackingState.CurrentAttackState.playerAttack.StunDuration, attackingState.CurrentAttackState.playerAttack.KnockbackStrenght);
            }
        }
    }
}


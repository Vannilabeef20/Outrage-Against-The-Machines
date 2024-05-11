using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game 
{
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private PlayerStateMachine stateMachine;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private PlayerSpecialParamsEvent specialParamsEvent;

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
            if (other.gameObject.TryGetComponent<IDamageble>(out IDamageble damageble))
            {
                float damage = stateMachine.Attacking.CurrentAttackState.playerAttack.Damage;
                float formerSpecialPercent = stateMachine.Attacking.specialChargeAmount / stateMachine.Attacking.MaxSpecialChargeAmount;

                stateMachine.Attacking.specialChargeAmount = Mathf.Clamp(stateMachine.Attacking.specialChargeAmount +
                    damage, stateMachine.playerInput.playerIndex, stateMachine.Attacking.MaxSpecialChargeAmount);

                float newSpecialPercent = stateMachine.Attacking.specialChargeAmount / stateMachine.Attacking.MaxSpecialChargeAmount;
                specialParamsEvent.Raise(this, new PlayerSpecialParams(stateMachine.playerInput.playerIndex, formerSpecialPercent, newSpecialPercent));
                damageble.TakeDamage(transform.position, damage, stateMachine.Attacking.CurrentAttackState.playerAttack.StunDuration,
                    stateMachine.Attacking.CurrentAttackState.playerAttack.KnockbackStrenght);
            }
        }
    }
}


using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Deals damage OnTriggerEnter to entities with given layerMasks.
    /// </summary>
	public class DamageCollider : MonoBehaviour
	{
        [field: Header("PARAMETERS"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] LayerMask playerMask;
        [SerializeField] LayerMask enemyMask;

        [SerializeField] float damage;

        [SerializeField] EParameterMode mode;

        [ShowIf("mode", EParameterMode.Player)]
        [SerializeField, Expandable] PlayerAttackSO playerAttackSO;

        [ShowIf("mode", EParameterMode.Enemy)]
        [SerializeField, Expandable] EnemyAttackSO enemyAttackSO;

        [Tooltip("How long (seconds) the entity will be stunned after taking damage.")]
        [ShowIf("mode", EParameterMode.Custom)]
        [SerializeField] float stunDuration;

        [Tooltip("How strong the force applied to the entity will be.")]
        [ShowIf("mode", EParameterMode.Custom)]
        [SerializeField] float knockbackStrenght;


        private void OnTriggerEnter(Collider other)
        {
            if (!playerMask.ContainsLayer(other.gameObject.layer) &&
                !enemyMask.ContainsLayer(other.gameObject.layer)) return;

            if (!other.TryGetComponent<IDamageble>(out IDamageble damageble)) return;

            switch(mode)
            {
                case EParameterMode.Player:
                    damageble.TakeDamage(transform.position, playerAttackSO.Damage,
                        playerAttackSO.StunDuration, playerAttackSO.KnockbackStrenght);
                    break;

                case EParameterMode.Enemy:
                    damageble.TakeDamage(transform.position, enemyAttackSO.Damage,
                        enemyAttackSO.StunDuration, enemyAttackSO.KnockbackStrenght);
                    break;

                case EParameterMode.Custom:
                    damageble.TakeDamage(transform.position, damage, stunDuration, knockbackStrenght);
                    break;
            }

            
        }

        enum EParameterMode
        {
            Custom,
            Player,
            Enemy,
        }
    }
}
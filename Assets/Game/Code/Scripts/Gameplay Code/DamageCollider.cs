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

        [Tooltip("How long (seconds) the entity will be stunned after taking damage.")]
        [SerializeField] float stunDuration;

        [Tooltip("How strong the force applied to the entity will be.")]
        [SerializeField] float knockbackStrenght;

        [Tooltip("Value to be multiplied to damage value if an enemy is damaged instead of a player.")]
        [SerializeField] float enemyMultiplier;

        private void OnTriggerEnter(Collider other)
        {
            if (playerMask.ContainsLayer(other.gameObject.layer))
            {
                if (other.TryGetComponent<IDamageble>(out IDamageble damageble))
                {
                    damageble.TakeDamage(transform.position, damage, stunDuration, knockbackStrenght);
                }
            }
            else if(enemyMask.ContainsLayer(other.gameObject.layer))
            {
                if (other.TryGetComponent<IDamageble>(out IDamageble damageble))
                {
                    damageble.TakeDamage(transform.position, damage * enemyMultiplier, stunDuration, knockbackStrenght);
                }
            }
        }
    }
}
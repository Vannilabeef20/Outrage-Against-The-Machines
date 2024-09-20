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


        private void OnTriggerEnter(Collider other)
        {
            if (!playerMask.ContainsLayer(other.gameObject.layer) &&
                !enemyMask.ContainsLayer(other.gameObject.layer)) return;

            if (!other.TryGetComponent<IDamageble>(out IDamageble damageble)) return;

            damageble.TakeDamage(transform.position, damage, stunDuration, knockbackStrenght);
        }
    }
}
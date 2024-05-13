using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class DamageCollider : MonoBehaviour
	{
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private float damage;
        [SerializeField] private float stunDuration;
        [SerializeField] private float knockbackStrenght;
        [SerializeField] private float enemyMultiplier;

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
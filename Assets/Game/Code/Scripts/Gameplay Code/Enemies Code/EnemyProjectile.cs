using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Creates a projectile behaviour on the applied object.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    public class EnemyProjectile : MonoBehaviour
    {
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]
        [Tooltip("The Rigidbody of the projectile.")]
        [SerializeField] Rigidbody body;

        [Tooltip("The gameobject that will follow the projectile as a shadow.")]
        [SerializeField, ShowAssetPreview] GameObject ShadowObject;

        [Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]
        [Tooltip("Layers for applying damage to a player.")]
        [SerializeField] LayerMask damagebleLayers;

        [Tooltip("How long the projectile will take to destroy itself after spawning.")]
        [SerializeField] float destroyDelay;

        [Tooltip("How much damage this projectile will do.")]
        [SerializeField, Min(0)] float damage;

        [Tooltip("How strong the hit knockback will be.")]
        [SerializeField, Min(0)] float knockBackstrength;

        [Tooltip("How long(seconds) the hit stun will be.")]
        [SerializeField, Min(0)] float stunDuration;

        [Tooltip("How fast the projectile will move.")]
        [SerializeField, Min(0)] float velocity;

        [Tooltip("Whether the projectile can be reflected.")]
        [SerializeField] bool reflectable;

        [Tooltip("Layers for reversing the projectile trajectory.")]
        [SerializeField, ShowIf("reflectable")] LayerMask reflectionLayers;

        [Tooltip("Extra layers added for damage after reflection.")]
        [SerializeField, ShowIf("reflectable")] LayerMask reflectionDamagebleLayers;

        [Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [Tooltip("Spawn position.")]
        [SerializeField, ReadOnly] Vector3 initialPos;

        [Tooltip("Whether the projectile has been reflected already.")]
        [SerializeField, ReadOnly, ShowIf("reflectable")] bool wasReflected;

        private void Awake()
        {
            initialPos = transform.position;
            body.velocity = velocity * transform.right;
            ShadowObject.transform.position = new Vector3(transform.position.x, transform.position.z, transform.position.z);
            Destroy(gameObject, destroyDelay);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (reflectable)
            {
                if (reflectionLayers.ContainsLayer(other.gameObject.layer))
                {
                    if(initialPos.x - transform.position.x > 0)
                    {
                        transform.right = Vector3.right;
                        body.velocity = velocity * Vector3.right;
                    }
                    else
                    {
                        transform.right = Vector3.left;
                        body.velocity = velocity * Vector3.left;
                    }
                    wasReflected = true;
                }
            }
            if(wasReflected)
            {
                if (reflectionDamagebleLayers.ContainsLayer(other.gameObject.layer))
                {
                    if (other.TryGetComponent<IDamageble>(out IDamageble damageble))
                    {
                        damageble.TakeDamage(transform.position, damage, stunDuration, knockBackstrength);
                    }
                }
            }
            else
            {
                if (damagebleLayers.ContainsLayer(other.gameObject.layer))
                {
                    if (other.TryGetComponent<IDamageble>(out IDamageble damageble))
                    {
                        damageble.TakeDamage(transform.position, damage, stunDuration, knockBackstrength);
                    }
                }
            }   
        }

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [SelectionBase]
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField, ShowAssetPreview] private GameObject ShadowObject;
        [SerializeField] private Rigidbody body;
        [SerializeField] private LayerMask playerLayers;
        [SerializeField] private LayerMask playerAttackLayers;
        [SerializeField] private float destroyDelay;
        [SerializeField] private bool reversible;
        [SerializeField, ReadOnly, ShowIf("reversible")] private bool wasReverted;
        [SerializeField, ShowIf("reversible")] private LayerMask enemyLayers;
        [SerializeField] private Vector3 initialPos;
        [Min(0)] public float damage;
        [Min(0)] public float knockBackstrength;
        [Min(0)] public float stunDuration;
        [Min(0)] public float velocity;
        private void Awake()
        {
            initialPos = transform.position;
            body.velocity = velocity * transform.right;
            ShadowObject.transform.position = new Vector3(transform.position.x, transform.position.z, transform.position.z);
            Destroy(gameObject, destroyDelay);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (reversible)
            {
                if (playerAttackLayers.ContainsLayer(other.gameObject.layer))
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
                    wasReverted = true;
                }
            }
            if(wasReverted)
            {
                if (enemyLayers.ContainsLayer(other.gameObject.layer))
                {
                    if (other.TryGetComponent<IDamageble>(out IDamageble damageble))
                    {
                        damageble.TakeDamage(transform.position, damage, stunDuration, knockBackstrength);
                    }
                }
            }
            else
            {
                if (playerLayers.ContainsLayer(other.gameObject.layer))
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


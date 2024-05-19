using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField, ShowAssetPreview] private GameObject ShadowObject;
        [SerializeField] private Rigidbody body;
        [SerializeField] private LayerMask hostileLayers;
        [SerializeField] private float destroyDelay;
        [Min(0)] public float damage;
        [Min(0)] public float knockBackstrength;
        [Min(0)] public float stunDuration;
        [Min(0)] public float velocity;
        private void Awake()
        {
            body.velocity = velocity * transform.right;
            ShadowObject.transform.position = new Vector3(transform.position.x, transform.position.z, transform.position.z);
            Destroy(gameObject, destroyDelay);
        }
        private void OnTriggerEnter(Collider other)
        {
            if(hostileLayers.ContainsLayer(other.gameObject.layer))
            {
                if (other.TryGetComponent<IDamageble>(out IDamageble damageble))
                {
                    damageble.TakeDamage(transform.position, damage, stunDuration, knockBackstrength);
                }
            }
        }

    }
}


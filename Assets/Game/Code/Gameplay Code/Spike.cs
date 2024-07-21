using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class Spike : MonoBehaviour
	{
        [SerializeField, ReadOnly] SpikeTrap trap;
        private void Awake()
        {
            trap = transform.parent.GetComponent<SpikeTrap>();
        }
        private void OnTriggerEnter(Collider other)
        {
            trap.DealDamage(other);
        }
    }
}
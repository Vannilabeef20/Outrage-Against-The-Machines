using UnityEngine;

namespace Game
{
    public class HealthDrop : ItemDrop
    {
        [SerializeField, Range(0f, 100f)] private float healPercent;

        protected override void ApplyPickupEffect(Collider other)
        {
            PlayerHealthHandler health = other.gameObject.GetComponentInChildren<PlayerHealthHandler>();
            if (health != null)
            {
                health.Heal(healPercent / 100);
            }
        }
    }
}

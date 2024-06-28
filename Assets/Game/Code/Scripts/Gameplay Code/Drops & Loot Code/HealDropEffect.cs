using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Can be added to a ItemDrop to apply a heal effect on pickup.
    /// </summary>
    [System.Serializable]
    public class HealDropEffect : BaseItemDropEffect
    {
        [Tooltip("Whether the heal effect will have a percentage component.")]
        [SerializeField] bool percent;
        [Tooltip("Whether the heal effect will have a flat(absoulte value) component.")]
        [SerializeField] bool flat;
        [Tooltip("How much percent healing will be applied.")]
        [SerializeField, AllowNesting, Range(0f, 100f), ShowIf("percent")] float healPercent;
        [Tooltip("How many flat healing points will be applied.")]
        [SerializeField, AllowNesting, Min(0), ShowIf("flat")] float healFlat;
        public override void ApplyEffect(GameObject targetPlayer)
        {
            PlayerHealthHandler health = targetPlayer.GetComponentInChildren<PlayerHealthHandler>();
            if (health == null)
            {
                return;
            }
            switch (percent, flat) //Ensures that percent and flat effects will only be applied when true
            {
                case (true, true):
                    health.Heal(healPercent, healFlat);
                    break;
                case (true, false):
                    health.Heal(healPercent, 0f);
                    break;
                case (false, true):
                    health.Heal(0f, healFlat);
                    break;
                case (false, false):
                    Debug.LogWarning("Heal drop has neither Flat or Percent flags active.");
                    break;
            }
        }
    }
}
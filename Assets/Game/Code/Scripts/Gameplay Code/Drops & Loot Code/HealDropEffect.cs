using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class HealDropEffect : BaseItemDropEffect
    {
        [SerializeField] private bool percent;
        [SerializeField] private bool flat;
        [SerializeField, Range(0f, 100f), ShowIf("percent")] private float healPercent;
        [SerializeField, Min(0), ShowIf("flat")] private float healFlat;
        public override void ApplyEffect(GameObject targetPlayer)
        {
            PlayerHealthHandler health = targetPlayer.GetComponentInChildren<PlayerHealthHandler>();
            if (health == null)
            {
                return;
            }
            switch (percent, flat)
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
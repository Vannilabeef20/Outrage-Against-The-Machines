using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Can be added to a ItemDrop to apply an energy build-up effect on pickup.
    /// </summary>
    [System.Serializable]
    public class EnergyDropEffect : BaseItemDropEffect
    {
        [Tooltip("How many special charge (Energy) points will be awarded once picked up")]
        [SerializeField] float specialCharge;
        public override void ApplyEffect(GameObject targetPlayer)
        {
            PlayerAttackingState attackingState = targetPlayer.GetComponentInChildren<PlayerStateMachine>().Attacking;
            if (attackingState == null)
            {
                Debug.LogWarning("No attackingState detected");
                return;
            }
            attackingState.AddSpecialCharges(specialCharge, true);
        }
    }
}
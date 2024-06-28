using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Can be added to a ItemDrop to apply add life points on pickup.
    /// </summary>
    [System.Serializable]
    public class LifeDropEffect : BaseItemDropEffect
    {
        [Tooltip("How many extra lifes will be awarded.")]
        [SerializeField, Min(0f)] private int lifeAmount = 1;
        public override void ApplyEffect(GameObject targetPlayer)
        {
            GameManager.Instance.TakeAddLife(lifeAmount);
        }
    }
}
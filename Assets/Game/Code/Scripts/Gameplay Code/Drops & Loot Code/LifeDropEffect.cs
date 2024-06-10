using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class LifeDropEffect : BaseItemDropEffect
    {
        [SerializeField, Min(0f)] private int lifeAmount;
        public override void ApplyEffect(GameObject targetPlayer)
        {
            GameManager.Instance.TakeAddLife(lifeAmount);
        }
    }
}
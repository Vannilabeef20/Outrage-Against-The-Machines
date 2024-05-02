using UnityEngine;


namespace Game
{
    public class LifeDrop : ItemDrop
    {
        [SerializeField, Min(0)] private int lifeAmount;
        protected override void ApplyPickupEffect(Collider other)
        {
            GameplayManager.Instance.TakeAddLife(lifeAmount);
        }
    }
}

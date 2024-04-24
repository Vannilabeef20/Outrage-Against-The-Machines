using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

namespace Game
{
    public abstract class ItemDrop : MonoBehaviour
    {
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private Transform itemSpriteTransform;
        private Vector3 floatRange = new Vector3 (0f,0.2f,0f);
        private float floatDuration = 1.5f;
        private Ease floatEase = Ease.InOutSine;

        private void Start()
        {
            itemSpriteTransform.DOMove(floatRange + itemSpriteTransform.position,
                floatDuration * 0.5f).SetEase(floatEase).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerMask.ContainsLayer(other.gameObject.layer))
            {
                AudioManager.instance.PlaySfxGlobal(pickupSound);
                ApplyPickupEffect(other);
                Destroy(gameObject);
            }
        }

        protected abstract void ApplyPickupEffect(Collider other);
    }
}

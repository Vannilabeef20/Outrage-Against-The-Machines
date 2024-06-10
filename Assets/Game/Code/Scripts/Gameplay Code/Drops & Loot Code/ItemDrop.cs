using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

namespace Game
{
    public class ItemDrop : MonoBehaviour
    {
        [SerializeField, Tag] private string playerTag;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private Transform itemSpriteTransform;
        [SerializeField] private BaseItemDropEffect[] pickupEffects;
        readonly private Vector3 floatRange = new Vector3 (0f,0.2f,0f);
        readonly private float floatDuration = 1.5f;
        readonly private Ease floatEase = Ease.InOutSine;

        private void Start()
        {
            itemSpriteTransform.DOMove(floatRange + itemSpriteTransform.position,
                floatDuration * 0.5f).SetEase(floatEase).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(playerTag))
            {
                AudioManager.instance.PlaySfxGlobal(pickupSound);
                foreach(var effect in pickupEffects)
                {
                    effect.ApplyEffect(other.gameObject);
                }
                DOTween.Kill(gameObject.transform);
                Destroy(gameObject);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Game
{
    public class BaseUIInteractive : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        [SerializeField] protected float selectScalePercentage = 0.15f;
        [SerializeField] protected float selectActivateDelay = 0.3f;

        [SerializeField] private float clickScalePercentage = 0.25f;
        [SerializeField] private float clickActivateDelay = 0.25f;

        public void AnimateInteractible(float changePercentage, float totalDuration, bool Loop = false, bool ScaleUp = true)
        {
            switch (Loop, ScaleUp)
            {
                case (false, false):
                    transform.DOScale(1f - changePercentage, totalDuration * 0.5f).SetEase(Ease.InOutBounce).SetUpdate(true).OnComplete(() => {
                        transform.DOScale(1f, totalDuration * 0.5f).SetEase(Ease.InOutBounce).SetUpdate(true);
                    });
                    break;
                case (false, true):
                    transform.DOScale(1f + changePercentage, totalDuration * 0.5f).SetEase(Ease.InOutBounce).SetUpdate(true).OnComplete(() => {
                        transform.DOScale(1f, totalDuration * 0.5f).SetEase(Ease.InOutBounce).SetUpdate(true);
                    });
                    break;
                default:


                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AnimateInteractible(0.15f, 0.3f);
        }

        public void OnSelect(BaseEventData eventData)
        {
            AudioManager.instance.PlayUiSelectSfx();
            AnimateInteractible(0.15f, 0.3f);
        }

        protected void PlayInteractionAnimation()
        {
            AnimateInteractible(clickScalePercentage, clickActivateDelay, ScaleUp: false);
        }
    }
}

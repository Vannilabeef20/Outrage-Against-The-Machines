using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Game
{
    public class MenuButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        [SerializeField] protected float selectScalePercentage = 0.15f;
        [SerializeField] protected float selectActivateDelay = 0.3f;

        [SerializeField] private float clickScalePercentage = 0.25f;
        [SerializeField] private float clickActivateDelay = 0.25f;

        public void AnimateInteractible(bool ScaleUp, float changePercentage, float totalDuration)
        {
            if (ScaleUp)
            {
                transform.DOScale(1f + changePercentage, totalDuration * 0.5f).SetEase(Ease.InOutBounce).SetUpdate(true).OnComplete(() =>
                transform.DOScale(1f, totalDuration * 0.5f).SetEase(Ease.InOutBounce).SetUpdate(true));
            }
            else
            {
                transform.DOScale(1f - changePercentage, totalDuration * 0.5f).SetEase(Ease.InOutBounce).SetUpdate(true).OnComplete(() =>
                transform.DOScale(1f, totalDuration * 0.5f).SetEase(Ease.InOutBounce).SetUpdate(true));
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AnimateInteractible(true, 0.15f, 0.3f);
        }

        public void OnSelect(BaseEventData eventData)
        {
            AudioManager.instance.PlayUiSelectSfx();
            AnimateInteractible(true, 0.15f, 0.3f);
        }

        protected void PlayInteractionAnimation()
        {
            AnimateInteractible(false, clickScalePercentage, clickActivateDelay);
        }
    }


}

[System.Flags]
public enum MenuId
{
    None = 0,
    StartMenu = 2,
    StartMenuOptions = 4,
    PauseMenu = 8,
    PauseOptionsMenu = 16,
    CharacterSelectionMenu = 32

}

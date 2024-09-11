using UnityEngine;

namespace Game
{
    public class ResumeButton : BaseUIInteractive
    {
        [SerializeField] private MenuIdEvent OnSetMenuVisibility;
        [SerializeField] private EMenuId TargetMenuIds;
        public void CallHidePauseMenu()
        {
            PlayInteractionAnimation();
            AudioManager.instance.PlayUiClickSfx();
            OnSetMenuVisibility?.Raise(this, TargetMenuIds);
        }
    }
}

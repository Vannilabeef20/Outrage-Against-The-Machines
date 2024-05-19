using UnityEngine;

namespace Game
{
    public class ResumeButton : MenuButton
    {
        [SerializeField] private MenuIdEvent OnSetMenuVisibility;
        [SerializeField] private MenuId TargetMenuIds;
        public void CallHidePauseMenu()
        {
            PlayInteractionAnimation();
            AudioManager.instance.PlayUiClickSfx();
            OnSetMenuVisibility?.Raise(this, TargetMenuIds);
        }
    }
}

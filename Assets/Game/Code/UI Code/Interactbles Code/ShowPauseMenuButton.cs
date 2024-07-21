using UnityEngine;

namespace Game
{
    public class ShowPauseMenuButton : MenuButton
    {
        [SerializeField] private MenuIdEvent OnSetMenuVisibility;
        [SerializeField] private MenuId TargetMenuIds;
        public void CallShowPauseMenu()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            OnSetMenuVisibility?.Raise(this,TargetMenuIds);
        } 
    }
}

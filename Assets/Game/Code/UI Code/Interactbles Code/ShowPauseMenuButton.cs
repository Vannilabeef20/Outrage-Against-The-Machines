using UnityEngine;

namespace Game
{
    public class ShowPauseMenuButton : BaseUIInteractive
    {
        [SerializeField] private MenuIdEvent OnSetMenuVisibility;
        [SerializeField] private EMenuId TargetMenuIds;
        public void CallShowPauseMenu()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            OnSetMenuVisibility.Raise(this,TargetMenuIds);
        }
    }
}

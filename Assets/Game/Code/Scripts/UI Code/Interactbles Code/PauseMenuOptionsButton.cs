using UnityEngine;

namespace Game
{
    public class PauseMenuOptionsButton : MenuButton
    {
        [SerializeField] private MenuIdEvent OnShowPauseMenuOptions;
        [SerializeField] private MenuId targetMenuIds;

        public void CallShowPauseMenuOptions()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            OnShowPauseMenuOptions?.Raise(this,targetMenuIds);
        }
    }
}


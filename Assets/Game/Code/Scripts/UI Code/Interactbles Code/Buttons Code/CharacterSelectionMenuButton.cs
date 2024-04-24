using UnityEngine;

namespace Game
{
    public class CharacterSelectionMenuButton : MenuButton
    {
        [SerializeField] private MenuIdEvent OnShowCharacterSelection;
        [SerializeField] private MenuId targetMenuIds;

        public void CallShowChacterSelection()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            OnShowCharacterSelection?.Raise(this, targetMenuIds);
        }
    }
}

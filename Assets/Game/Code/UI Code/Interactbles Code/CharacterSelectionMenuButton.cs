using UnityEngine;

namespace Game
{
    public class CharacterSelectionMenuButton : BaseUIInteractive
    {
        [SerializeField] private MenuIdEvent OnShowCharacterSelection;
        [SerializeField] private EMenuId targetMenuIds;

        public void CallShowChacterSelection()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            OnShowCharacterSelection?.Raise(this, targetMenuIds);
        }
    }
}

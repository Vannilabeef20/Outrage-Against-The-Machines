using UnityEngine;

namespace Game
{
    public class CharacterSelectionMenuButton : BaseUIInteractive
    {
        [SerializeField] MenuIdEvent OnShowCharacterSelection;
        [SerializeField] EMenuId targetMenuIds;
        [SerializeField] Transition transition;

        public void CallShowChacterSelection()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();

            TransitionManager.Instance.LoadScreen(targetMenuIds, transition);
        }
    }
}

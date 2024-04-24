using UnityEngine;

namespace Game
{
    public class QuitButton : MenuButton
    {
        public void CallQuitGame()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            Time.timeScale = 1;
            Application.Quit();
        }
    }
}

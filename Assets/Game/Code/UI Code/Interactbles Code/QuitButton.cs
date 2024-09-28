using UnityEngine;

namespace Game
{
    public class QuitButton : BaseUIInteractive
    {
        public void CallQuitGame()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            Time.timeScale = 1;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif

        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class QuitButton : BaseUIInteractive
    {
#if UNITY_WEBGL
        private void Awake()
        {
            GetComponent<Button>().interactable = false;
        }
#endif

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

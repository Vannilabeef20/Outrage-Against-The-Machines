using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// Creates a button that OnClick, tell Gamemanager to load a new scene.
    /// </summary>
    public class LoadSceneButton : BaseUIInteractive 
    {
        [Scene,SerializeField] int targetScene;

        public void LoadScene()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            TransitionManager.Instance.LoadScene(targetScene);
        }
    }
}

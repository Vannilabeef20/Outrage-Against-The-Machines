using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LoadSceneButton : MenuButton 
    {
        [Scene,SerializeField] private int targetScene;
        [SerializeField] private IntEvent loadSceneEvent;

        public void LoadScene()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();
            loadSceneEvent.Raise(this, targetScene);
        }
    }
}

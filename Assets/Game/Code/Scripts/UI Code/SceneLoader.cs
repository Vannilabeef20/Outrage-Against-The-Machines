using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using DG.Tweening;

namespace Game {
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Canvas LoadingCanvas;
        [SerializeField] private Image LoadingBg;
        [SerializeField] private Image LoadingBar;
        [SerializeField] private Image LoadingBarBackground;

        [SerializeField] private float transitionDuration;
        [SerializeField] private Ease transitionEasing;
        [SerializeField, ReadOnly] private float loadingProgress;

        private AsyncOperation loadingOperation;
        public void LoadScene(int sceneIndex)
        {
            StartCoroutine(LoadLevelAsync(sceneIndex));
        }
        public IEnumerator LoadLevelAsync(int level)
        {
            LoadingCanvas.enabled = true;
            yield return new WaitForSecondsRealtime(transitionDuration);
            loadingOperation = SceneManager.LoadSceneAsync(level);

            while (loadingOperation.isDone != true)
            {
                loadingProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
                LoadingBar.fillAmount = loadingProgress;

                yield return null;
            }
        }
        private void Start()
        {
            LoadingCanvas.enabled = false;
        }
    }
}

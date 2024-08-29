using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;


namespace Game
{
    public class FullScreenToggle : BaseUIInteractive
    {
        [SerializeField] Image toggleImage;
        [SerializeField, ReadOnly] const string PlayerPrefsFullScreen = "FullscreenValue";
        [SerializeField, ReadOnly] private bool active;
        [SerializeField, ShowAssetPreview] private Sprite[] toggleSprites;

        private void Awake()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsFullScreen) == 1) // 1 means true, 0 false
            {
                active = true;
                toggleImage.sprite = toggleSprites[1];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 1);
            }
            else
            {
                active = false;
                toggleImage.sprite = toggleSprites[0];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 0);
            }
        }
        private void OnEnable()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsFullScreen) == 1)
            {
                active = true;
                toggleImage.sprite = toggleSprites[1];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 1);
            }
            else
            {
                active = false;
                toggleImage.sprite = toggleSprites[0];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 0);
            }
        }
        public void SetFullScreen()
        {
            PlayInteractionAnimation();
            AudioManager.instance.PlayUiClickSfx();
            active = !active;
            Screen.fullScreen = active;
            if (active)
            {
                toggleImage.sprite = toggleSprites[1];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 1);
            }
            else
            {
                toggleImage.sprite = toggleSprites[0];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 0);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;


namespace Game
{
    public class FullScreenToggle : MenuButton
    {
        [SerializeField, ReadOnly] Image toggleImage;
        [SerializeField, ReadOnly] const string PlayerPrefsFullScreen = "FullscreenValue";
        [SerializeField, ReadOnly] private bool active;
        [SerializeField] private Sprite[] toggleSprites;

        private void Awake()
        {
            toggleImage = GetComponent<Image>();
        }
        private void OnEnable()
        {
            if (active)
            {
                toggleImage.sprite = toggleSprites[0];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 0);
            }
            else
            {
                toggleImage.sprite = toggleSprites[1];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 1);
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
                toggleImage.sprite = toggleSprites[0];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 0);
            }
            else
            {
                toggleImage.sprite = toggleSprites[1];
                PlayerPrefs.SetInt(PlayerPrefsFullScreen, 1);
            }
        }
    }
}

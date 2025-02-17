using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;


namespace Game
{
	public class ScreenShakeToggle : BaseUIInteractive
	{
        [SerializeField] Image toggleImage;
        [SerializeField] BoolEvent screenShakeToggleEvent;
        [SerializeField, ReadOnly] const string PlayerPrefsScreenShake = "ScreenShake";
        [SerializeField, ReadOnly] bool active;
        [SerializeField, ShowAssetPreview] Sprite[] toggleSprites;

        private void Awake()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsScreenShake) == 1) // 1 means true, 0 false
            {
                active = true;
                toggleImage.sprite = toggleSprites[1];
                PlayerPrefs.SetInt(PlayerPrefsScreenShake, 1);
            }
            else
            {
                active = false;
                toggleImage.sprite = toggleSprites[0];
                PlayerPrefs.SetInt(PlayerPrefsScreenShake, 0);
            }
            screenShakeToggleEvent.Raise(this, active);
        }
        private void OnEnable()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsScreenShake) == 1)
            {
                active = true;
                toggleImage.sprite = toggleSprites[1];
                PlayerPrefs.SetInt(PlayerPrefsScreenShake, 1);
            }
            else
            {
                active = false;
                toggleImage.sprite = toggleSprites[0];
                PlayerPrefs.SetInt(PlayerPrefsScreenShake, 0);
            }
            screenShakeToggleEvent.Raise(this, active);
        }
        public void _SetScreenShake()
        {
            AudioManager.instance.PlayUiClickSfx();
            active = !active;
            if (active)
            {
                toggleImage.sprite = toggleSprites[1];
                PlayerPrefs.SetInt(PlayerPrefsScreenShake, 1);
            }
            else
            {
                toggleImage.sprite = toggleSprites[0];
                PlayerPrefs.SetInt(PlayerPrefsScreenShake, 0);
            }
            screenShakeToggleEvent.Raise(this, active);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;


namespace Game
{
    public class RumbleToggle : BaseUIInteractive
    {
        [SerializeField] Image toggleImage;
        [SerializeField] BoolEvent rumbleToggleEvent;
        const string PlayerPrefsScreenShake = "Rumble";
        [SerializeField, ReadOnly] bool active;
        [SerializeField, ShowAssetPreview] Sprite[] toggleSprites;

        private void Awake()
        {
#if UNITY_WEBGL
            GetComponent<Button>().interactable = false;
            return;
#endif
#pragma warning disable
            if (PlayerPrefs.GetInt(PlayerPrefsScreenShake, 1) == 1) // 1 means true, 0 false
#pragma warning enable
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
            rumbleToggleEvent.Raise(this, active);
        }
        private void OnEnable()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsScreenShake, 1) == 1)
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
            rumbleToggleEvent.Raise(this, active);
        }
        public void _SetRumble()
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
            rumbleToggleEvent.Raise(this, active);
        }
    }
}
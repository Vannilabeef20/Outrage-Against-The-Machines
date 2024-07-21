using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

namespace Game
{
    /// <summary>
    /// Creates a dropdown 
    /// </summary>
    public class ResolutionDropdown : MenuButton
    {
        [SerializeField, ReadOnly] TMP_Dropdown dropdown;
        [SerializeField, ReadOnly] Resolution[] unityResolutions;
        [SerializeField, ReadOnly] int currentResolutionIndex;
        [SerializeField, ReadOnly] const string playerPrefsFullScreen = "FullscreenValue";

        private void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }
        private void Start()
        {
            unityResolutions = Screen.resolutions;
            List<string> optionsText = new List<string>();
            for (int i = 0; i < unityResolutions.Length; i++)
            {
                string optionText = $"{unityResolutions[i].width} x {unityResolutions[i].height} {unityResolutions[i].refreshRateRatio.value.ToString("0.##")}Hz";
                optionsText.Add(optionText);
            }
            for (int i = 0; i < unityResolutions.Length; i++)
            {
                if (unityResolutions[i].width == Screen.currentResolution.width && unityResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
                dropdown.ClearOptions();
            dropdown.AddOptions(optionsText);
            dropdown.value = currentResolutionIndex;
            dropdown.RefreshShownValue();
        }

        public void SetScreenResolution(int ResolutionIndex)
        {
            PlayInteractionAnimation();
            AudioManager.instance.PlayUiClickSfx();
            bool fullScreen = PlayerPrefs.GetInt(playerPrefsFullScreen) == 0;
            Screen.SetResolution(unityResolutions[ResolutionIndex].width, unityResolutions[ResolutionIndex].height, fullScreen);
            Screen.fullScreen = (fullScreen);
        }
    }
}

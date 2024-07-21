using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Audio;

namespace Game
{
    public class VolumeSlider : MenuButton
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField, ReadOnly] private Slider slider;
        [SerializeField, Dropdown("mixerGroupNames")] private string mixerGroupName;
        [SerializeField, ReadOnly] private string[] mixerGroupNames = new string[] {"MasterVolume", "MusicVolume", "SfxVolume" };
        private string _audioMixerPath ="AudioResources/Mixer";
        private void Awake()
        {
            slider = GetComponent<Slider>();
            slider.value = PlayerPrefs.GetFloat(mixerGroupName, 0.5f);
            mixer.SetFloat(mixerGroupName, Mathf.Log10(slider.value) * 20);
        }

        public void _SetVolume()
        {
            mixer.SetFloat(mixerGroupName, Mathf.Log10(slider.value) * 20);
            PlayerPrefs.SetFloat(mixerGroupName, slider.value);
        }
    }
}


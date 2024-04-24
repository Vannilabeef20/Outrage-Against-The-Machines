using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Audio;

namespace Game
{
    public class VolumeSlider : MenuButton
    {
        [SerializeField, ReadOnly] private Slider slider;
        [SerializeField, ReadOnly] private AudioMixer mixer;
        [SerializeField, Dropdown("mixerGroupNames")] private string mixerGroupName;
        [SerializeField, ReadOnly] private string[] mixerGroupNames = new string[] {"MasterVolume", "MusicVolume", "SfxVolume" };
        private string _audioMixerPath ="AudioResources/Mixer";
        private void Awake()
        {
            mixer = Resources.Load<AudioMixer>(_audioMixerPath);
            slider = GetComponent<Slider>();
            if (mixer == null)
            {
                DebugManager.instance.LogError(new DebugLogStruct(EDebugSubjectFlags.Test, this, "could not find appropiate mixer," +
                    " please check if current path is valid."));
            }
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


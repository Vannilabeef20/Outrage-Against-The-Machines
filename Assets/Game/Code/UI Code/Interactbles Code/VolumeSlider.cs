using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Audio;
using FMODUnity;
using FMOD.Studio;

namespace Game
{
    public class VolumeSlider : BaseUIInteractive
    {
        [SerializeField, ReadOnly] Slider slider;
        [SerializeField, Expandable] AudioVolumeCurveSO volumeCurve;
        [SerializeField, Dropdown("busNames")] string busGroup;
        [SerializeField, ReadOnly] string[] busNames = new string[] {"Master","Music","SFX"};
        Bus bus;

        private void Awake()
        {
            
            if(busGroup == busNames[0])
            {
                bus = RuntimeManager.GetBus("bus:/");
            }
            else
            {
                bus = RuntimeManager.GetBus($"bus:/Master/{busGroup}");
            }

            slider = GetComponent<Slider>();
            slider.value = PlayerPrefs.GetFloat(busGroup, 0.5f);
            _SetVolume();
        }

        public void _SetVolume()
        {
            float db = slider.value.Map(0f, 1f, -80f, 10f);
            float volume = Mathf.Pow(10f, db/ 20f);
            Debug.Log($"Volume: {volume} DB:{db}");
            //float volume = volumeCurve.Curve.Evaluate(slider.value);
            bus.setVolume(volume);
            PlayerPrefs.SetFloat(busGroup, slider.value);
        }
    }
}


using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Audio;
using FMODUnity;
using FMOD.Studio;

namespace Game
{
    public class VolumeSlider : MonoBehaviour
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
        }

        public void _SetVolume()
        {
            //  CURVA DB PARA LINEAR (o valor Y da curva é o valor dos decibéis)
            float db = volumeCurve.Curve.Evaluate(slider.value);
            float volume = Mathf.Pow(10f, db / 20f);
            bus.setVolume(volume);
            PlayerPrefs.SetFloat(busGroup, slider.value);
        }
    }
}


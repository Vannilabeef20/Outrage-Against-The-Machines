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
            //O valor X da curva tem de ir obrigatóriamente de 0 á 1 (pois o slider vai de 0 á 1)

            //Teste e escolha o método que achar melhor (descomente os floats abaixo de um titúlo enquanto deixa os outros comentados

            //  LINEAR ANTIGO (sem curva, implementação atual)
            //float db = slider.value.Map(0f, 1f, -80f, 10f);
            //float volume = Mathf.Pow(10f, db/ 20f);

            //  LINEAR DIRETO NA CURVA  (o valor Y da curva é o valor da linear, sem conversão para DB)
            //float linear = volumeCurve.Curve.Evaluate(slider.value);
            //float volume = linear;

            //  CURVA DB PARA LINEAR (o valor Y da curva é o valor dos decibéis)
            float db = volumeCurve.Curve.Evaluate(slider.value);
            float volume = Mathf.Pow(10f, db / 20f);

            Debug.Log($"Volume: {volume} DB:{db}");
            bus.setVolume(volume);
            PlayerPrefs.SetFloat(busGroup, slider.value);
        }
    }
}


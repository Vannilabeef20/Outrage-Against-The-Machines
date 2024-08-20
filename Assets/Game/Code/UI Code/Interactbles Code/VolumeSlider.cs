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
            //O valor X da curva tem de ir obrigat�riamente de 0 � 1 (pois o slider vai de 0 � 1)

            //Teste e escolha o m�todo que achar melhor (descomente os floats abaixo de um tit�lo enquanto deixa os outros comentados

            //  LINEAR ANTIGO (sem curva, implementa��o atual)
            //float db = slider.value.Map(0f, 1f, -80f, 10f);
            //float volume = Mathf.Pow(10f, db/ 20f);

            //  LINEAR DIRETO NA CURVA  (o valor Y da curva � o valor da linear, sem convers�o para DB)
            //float linear = volumeCurve.Curve.Evaluate(slider.value);
            //float volume = linear;

            //  CURVA DB PARA LINEAR (o valor Y da curva � o valor dos decib�is)
            float db = volumeCurve.Curve.Evaluate(slider.value);
            float volume = Mathf.Pow(10f, db / 20f);

            Debug.Log($"Volume: {volume} DB:{db}");
            bus.setVolume(volume);
            PlayerPrefs.SetFloat(busGroup, slider.value);
        }
    }
}


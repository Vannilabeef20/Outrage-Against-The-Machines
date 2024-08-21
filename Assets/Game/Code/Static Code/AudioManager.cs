using UnityEngine;
using UnityEngine.Audio;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

namespace Game
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [Header("SFX"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] StudioEventEmitter[] uiClickPositiveEmitters;
        [SerializeField] StudioEventEmitter[] uiClickNegativeEmitters;
        [SerializeField] StudioEventEmitter[] uiSelectEmitters;
        [SerializeField, ReadOnly] int uiClickPositiveSoundIndex;
        [SerializeField, ReadOnly] int uiClickNegativeSoundIndex;
        [SerializeField, ReadOnly] int uiSelectSoundIndex;

        [Header("VOLUME INIT"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] AudioVolumeCurveSO volumeCurve;
        [SerializeField, ReadOnly] string[] busNames = new string[] { "Master", "Music", "SFX" };

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            Bus tempBus;
            foreach (var busName in busNames)
            {
                if (busName == busNames[0])
                {
                    tempBus = RuntimeManager.GetBus("bus:/");
                }
                else
                {
                    tempBus = RuntimeManager.GetBus($"bus:/Master/{busName}");
                }
                float db = volumeCurve.Curve.Evaluate(PlayerPrefs.GetFloat(busName, 0.5f));
                float volume = Mathf.Pow(10f, db / 20f);
                tempBus.setVolume(volume);
            }
        }
     
        public void PlayUiClickSfx()
        {
            if (uiClickPositiveSoundIndex >= uiClickPositiveEmitters.Length)
            {
                uiClickPositiveSoundIndex = 0;
            }
            uiClickPositiveEmitters[uiClickPositiveSoundIndex].Play();
            uiClickPositiveSoundIndex++;
        }
        public void PlayUiSelectSfx()
        {
            if(uiSelectSoundIndex >= uiSelectEmitters.Length)
            {
                uiSelectSoundIndex = 0;
            }
            uiClickNegativeEmitters[uiClickNegativeSoundIndex].Play();
            uiSelectSoundIndex++;
        }

        /*
        public void InitiateVolume(string mixerParamName, float value)
        {
            Mixer.SetFloat(mixerParamName, Mathf.Log10(value) * 20);
        }
        */
    }
}

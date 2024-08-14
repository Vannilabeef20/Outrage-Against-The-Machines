using UnityEngine;
using UnityEngine.Audio;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using FMODUnity;

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

using UnityEngine;
using UnityEngine.Audio;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

namespace Game
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] AudioMixer Mixer;
        [SerializeField] AudioSource MusicSource;
        [SerializeField] AudioSource SFXSource;
        [SerializeField, ReadOnly]
        private string MasterVolumeName = "MasterVolume";
        [SerializeField, ReadOnly]
        private string SfxVolumeName = "SfxVolume";
        [SerializeField, ReadOnly]
        private string MusicVolumeName = "MusicVolume";

        [Header("MUSIC"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] AudioClip[] sceneMusics;

        [Header("SFX"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] AudioClip[] uiClickPositiveSounds;
        [SerializeField] AudioClip[] uiClickNegativeSounds;
        [SerializeField] AudioClip[] uiSelectSounds;
        [SerializeField, ReadOnly] int uiClickPositiveSoundIndex;
        [SerializeField, ReadOnly] int uiClickNegativeSoundIndex;
        [SerializeField, ReadOnly] int uiSelectSoundIndex;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += PlaySceneMusic;
                MusicSource.clip = sceneMusics[SceneManager.GetActiveScene().buildIndex];
                MusicSource.Play();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            InitiateVolume(MasterVolumeName, PlayerPrefs.GetFloat(MasterVolumeName, 0.5f));
            InitiateVolume(SfxVolumeName, PlayerPrefs.GetFloat(SfxVolumeName, 0.5f));
            InitiateVolume(MusicVolumeName, PlayerPrefs.GetFloat(MusicVolumeName, 0.5f));
        }

        private void PlaySceneMusic(Scene scene, LoadSceneMode loadSceneMode)
        {
            MusicSource.Stop();
            MusicSource.clip = sceneMusics[scene.buildIndex];
            MusicSource.Play();
        }
     
        public void PlayUiClickSfx()
        {
            if (uiClickPositiveSoundIndex >= uiClickPositiveSounds.Length)
            {
                uiClickPositiveSoundIndex = 0;
            }
            SFXSource.PlayOneShot(uiClickPositiveSounds[uiClickPositiveSoundIndex]);
            uiClickPositiveSoundIndex++;
        }
        public void PlayUiSelectSfx()
        {
            if(uiSelectSoundIndex >= uiSelectSounds.Length)
            {
                uiSelectSoundIndex = 0;
            }
            SFXSource.PlayOneShot(uiSelectSounds[uiSelectSoundIndex]);
            uiSelectSoundIndex++;
        }

        public void InitiateVolume(string mixerParamName, float value)
        {
            Mixer.SetFloat(mixerParamName, Mathf.Log10(value) * 20);
        }
        public void PlaySfxGlobal(AudioClip sfx)
        {
            SFXSource.PlayOneShot(sfx);
        }
    }
}

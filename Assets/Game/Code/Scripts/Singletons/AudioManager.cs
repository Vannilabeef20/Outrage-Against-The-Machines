using UnityEngine;
using UnityEngine.Audio;
using NaughtyAttributes;

namespace Game
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        [Header("REFERENCES"), HorizontalLine(2f, EColor.White)]
        [SerializeField, ReadOnly] private AudioMixer Mixer;
        [SerializeField, ReadOnly] private AudioSource SFXSource;


        [SerializeField, ReadOnly, Foldout("Sounds")] private AudioClip[] uiClickPositiveSounds;
        [SerializeField, ReadOnly, Foldout("Sounds")] private int uiClickPositiveSoundIndex;
        [SerializeField, ReadOnly, Foldout("Sounds")] private AudioClip[] uiClickNegativeSounds;
        [SerializeField, ReadOnly, Foldout("Sounds")] private int uiClickNegativeSoundIndex;
        [SerializeField, ReadOnly, Foldout("Sounds")] private AudioClip[] uiSelectSounds;
        [SerializeField, ReadOnly, Foldout("Sounds")] private int uiSelectSoundIndex;


        private string _audioMixerPath = "AudioResources/Mixer";

        private string _uiSelectSoundsPath = "AudioResources/UI_SelectMenuSounds";

        private string _uiClickPositiveSoundsPath = "AudioResources/UI_ClickPositiveMenuSounds";
        [SerializeField, ReadOnly, Foldout("Mixer names")]
        private string MasterVolumeName = "MasterVolume";
        [SerializeField, ReadOnly, Foldout("Mixer names")]
        private string SfxVolumeName = "SfxVolume";
        [SerializeField, ReadOnly, Foldout("Mixer names")]
        private string MusicVolumeName = "MusicVolume";


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
            Mixer = Resources.Load<AudioMixer>(_audioMixerPath);
            uiSelectSounds = Resources.LoadAll<AudioClip>(_uiSelectSoundsPath);
            uiClickPositiveSounds = Resources.LoadAll<AudioClip>(_uiClickPositiveSoundsPath);
            SFXSource = GetComponentInChildren<AudioSource>();
        }
        private void Start()
        {
            InitiateVolume(MasterVolumeName, PlayerPrefs.GetFloat(MasterVolumeName, 0.5f));
            InitiateVolume(SfxVolumeName, PlayerPrefs.GetFloat(SfxVolumeName, 0.5f));
            InitiateVolume(MusicVolumeName, PlayerPrefs.GetFloat(MusicVolumeName, 0.5f));
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

using UnityEngine;
using FMOD;
using FMODUnity;
namespace Game
{
    public class SceneryAudio : MonoBehaviour
    {
        [SerializeField] bool testando;
        [SerializeField] StudioEventEmitter fmodEmitter;
        [SerializeField] private AudioSource source;

        public void EnableDisableAudio(MenuId id)
        {
            if (id == MenuId.None)
            {
                if(testando)
                fmodEmitter.Stop();
                source.UnPause();
            }
            else
            {
                if(testando)
                fmodEmitter.Play();
                source.Pause();
            }
        }
        private void Update()
        {
            if (source.maxDistance < Vector3.Distance(GameManager.Instance.MainCamera.transform.position, transform.position))
            {
                source.Pause();
                return;
            }
            source.UnPause();
        }
    }
}

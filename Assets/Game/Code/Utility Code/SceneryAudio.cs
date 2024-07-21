using UnityEngine;
namespace Game
{
    public class SceneryAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource source;

        public void EnableDisableAudio(MenuId id)
        {
            if (id == MenuId.None)
            {
                source.UnPause();
            }
            else
            {
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

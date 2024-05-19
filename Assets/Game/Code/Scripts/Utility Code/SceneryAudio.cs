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
    }
}

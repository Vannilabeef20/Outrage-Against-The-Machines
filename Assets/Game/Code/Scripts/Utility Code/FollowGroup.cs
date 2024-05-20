using UnityEngine;
using Cinemachine;

namespace Game
{
    public class FollowGroup : MonoBehaviour
    {
        private void Awake()
        {
            FindObjectOfType<CinemachineVirtualCamera>().Follow = gameObject.transform;
        }
        private void Update()
        {
            Vector3 sum = Vector3.zero;
            int amount = 0;
            foreach (var player in GameManager.Instance.PlayerCharacterList)
            {
                if(player.GameObject == null)
                {
                    continue;
                }
                if (!player.isPlayerActive)
                {
                    continue;
                }
                sum += player.GameObject.transform.position;
                amount++;
            }
            if(sum == Vector3.zero || amount == 0)
            {
                return;
            }
            transform.position = sum / amount;
        }
    }
}

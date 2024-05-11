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
            for(int i = 0; i < GameManager.Instance.PlayerObjectArray.Length; i++)
            {
                if (!GameManager.Instance.isPlayerActiveArray[i])
                {
                    continue;
                }
                sum += GameManager.Instance.PlayerObjectArray[i].transform.position;
                amount++;
            }       
            transform.position = sum / amount;
        }
    }
}

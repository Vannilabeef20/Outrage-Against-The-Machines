using UnityEngine;

namespace Game
{
    public class FollowGroup : MonoBehaviour
    {
        private void Update()
        {
            Vector3 sum = Vector3.zero;
            foreach (var obj in GameManager.Instance.PlayerObjectArray)
            {
                sum += obj.transform.position;
            }
            transform.position = sum / GameManager.Instance.PlayerObjectArray.Length;
        }
    }
}

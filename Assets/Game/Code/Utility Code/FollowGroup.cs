using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Creates a central point between all *player* marked active in the *GameManager* for easier camera follow.
    /// </summary>
    public class FollowGroup : MonoBehaviour
    {
        [SerializeField, ReadOnly] List<Transform> playerTransformList;
        private void Awake()
        {
            FindObjectOfType<CinemachineVirtualCamera>().Follow = gameObject.transform;
        }
        private void Update()
        {
            playerTransformList.Clear();
            Vector3 sum = Vector3.zero;
            foreach (var player in GameManager.Instance.PlayerCharacterList)
            {
                if (player.Transform == null) continue;

                if (!player.isPlayerActive) continue;

                sum += player.Transform.position;
                playerTransformList.Add(player.Transform);
            }

            if (sum == Vector3.zero || playerTransformList.Count < 1) return;

            transform.position = sum / playerTransformList.Count;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if(playerTransformList.Count < 1)
            {
                return;
            }
            foreach(var player in playerTransformList)
            {
                Debug.DrawLine(player.position, transform.position);
            }
        }
#endif
    }
}

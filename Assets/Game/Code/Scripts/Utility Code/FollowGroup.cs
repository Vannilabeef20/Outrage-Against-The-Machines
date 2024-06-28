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
        [SerializeField, ReadOnly, TextArea] string DESCRIPTION =
            "Creates a central point between all *player* marked active in the *GameManager* for easier camera follow.";
        [SerializeField, ReadOnly] List<GameObject> playerList;
        private void Awake()
        {
            FindObjectOfType<CinemachineVirtualCamera>().Follow = gameObject.transform;
        }
        private void Update()
        {
            playerList.Clear();
            Vector3 sum = Vector3.zero;
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
                playerList.Add(player.GameObject);
            }
            if(sum == Vector3.zero || playerList.Count < 1)
            {
                return;
            }
            transform.position = sum / playerList.Count;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if(playerList.Count < 1)
            {
                return;
            }
            foreach(GameObject player in playerList)
            {
                Debug.DrawLine(player.transform.position, transform.position);
            }
        }
#endif
    }
}

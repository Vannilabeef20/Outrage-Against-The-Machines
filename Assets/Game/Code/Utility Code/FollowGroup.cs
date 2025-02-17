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
        public static FollowGroup Instance;
        [SerializeField, ReadOnly] List<Transform> targetList;
        Vector3 sum;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            FindAnyObjectByType<CinemachineVirtualCamera>().Follow = gameObject.transform;
        }
        private void Update()
        {
            sum = Vector3.zero;
            foreach (var target in targetList)
            {
                sum += target.transform.position;
            }

            if (sum == Vector3.zero || targetList.Count < 1) return;

            transform.position = sum / targetList.Count;
        }

        public void AddTarget(Transform target)
        {
            if(targetList.Contains(target)) return;
            targetList.Add(target);
        }

        public void RemoveTarget(Transform target)
        {
            targetList.Remove(target);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (targetList.Count < 1) return;

            foreach(var player in targetList)
            {
                Debug.DrawLine(player.position, transform.position);
            }

            Gizmos.DrawSphere(sum / targetList.Count, 0.1f);
        }
#endif
    }
}

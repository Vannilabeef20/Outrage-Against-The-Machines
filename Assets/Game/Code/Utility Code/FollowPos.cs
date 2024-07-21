using UnityEngine;
using NaughtyAttributes;

namespace Game 
{
    public class FollowPos : MonoBehaviour
    {
        [System.Flags]
        private enum ETargetAxis
        {
            X = 1,
            Y = 2,
            Z = 4
        }

        [SerializeField] private Transform followTransform;
        [SerializeField] private ETargetAxis targetAxis;
        [SerializeField, ReadOnly] private Vector3 lastPos;
        [SerializeField, ReadOnly] private Vector3 currentTargetPos;
        public bool active = true;
        public bool centerOnStart;

        private void Start()
        {
            if (targetAxis.HasFlag(ETargetAxis.X))
            {
                lastPos = new Vector3(followTransform.position.x, lastPos.y, lastPos.z);
            }
            if (targetAxis.HasFlag(ETargetAxis.Y))
            {
                lastPos = new Vector3(lastPos.x, followTransform.position.y, lastPos.z);
            }
            if (targetAxis.HasFlag(ETargetAxis.Z))
            {
                lastPos = new Vector3(lastPos.x, lastPos.y, followTransform.position.z);
            }
            if (centerOnStart)
            {
                CenterOnTarget();
            }
        }
        private void Update()
        {
            if (!active)
            {
                return;
            }
            currentTargetPos = Vector3.zero;
            if (targetAxis.HasFlag(ETargetAxis.X))
            {
                currentTargetPos = new Vector3(followTransform.position.x, currentTargetPos.y, currentTargetPos.z);
            }
            if (targetAxis.HasFlag(ETargetAxis.Y))
            {
                currentTargetPos = new Vector3(currentTargetPos.x, followTransform.position.y, currentTargetPos.z);
            }
            if (targetAxis.HasFlag(ETargetAxis.Z))
            {
                currentTargetPos = new Vector3(currentTargetPos.x, currentTargetPos.y, followTransform.position.z);
            }
            transform.position += currentTargetPos - lastPos;
            lastPos = currentTargetPos;
        }

        [Button("Center")]
        public void CenterOnTarget()
        {
            if (targetAxis.HasFlag(ETargetAxis.X))
            {
                transform.position = new Vector3(followTransform.position.x, transform.position.y, transform.position.z);
            }
            if (targetAxis.HasFlag(ETargetAxis.Y))
            {
                transform.position = new Vector3(transform.position.x, followTransform.position.y, transform.position.z);
            }
            if (targetAxis.HasFlag(ETargetAxis.Z))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, followTransform.position.z);
            }
        }
    }
}


using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class MoveContextSteering : BasePathfinding
    {
        [SerializeField] private float maxAvoidanceRadius;
        [SerializeField] private LayerMask ObstacleLayerMask;
        [SerializeField, Range(8,64)] private int numRays;
        [SerializeField] private float obstacleDetectionRadius;
        [SerializeField, ReadOnly] private Vector3[] rayDirections;
        [SerializeField, ReadOnly] private float[] interestValues;
        [SerializeField, ReadOnly] private float[] obstacleValues;
        [SerializeField, ReadOnly] private Vector3 normal = new Vector3(0,1,-1);
        private void Awake()
        {
            rayDirections = new Vector3[numRays];
            interestValues = new float[numRays];
            obstacleValues = new float[numRays];
            for (int i = 0; i < numRays; i++)
            {
                float angle = 2 * Mathf.PI/ numRays * i;
                angle *= 180 / Mathf.PI;
                rayDirections[i] = Quaternion.Euler(0,0,angle) * Vector3.up;
                rayDirections[i] = new Vector3(rayDirections[i].x,rayDirections[i].y, rayDirections[i].y);
                rayDirections[i].Normalize();
            }
        }
        public override Vector3 GetMovementDirection(Vector3 targetPosition, bool IsOnScreen)
        { 
            return CalculateContextSteering(CalculateTargetDirection(targetPosition), IsOnScreen);
        }

        public virtual Vector3 CalculateTargetDirection(Vector3 targetPosition)
        {
            Vector3 targetDirection = targetPosition - body.position;
            targetDirection.Normalize();
            return targetDirection;
        }

        private Vector3 CalculateContextSteering(Vector3 targetDirection, bool isOnScreen)
        {
            for (int i = 0; i < rayDirections.Length; i++)
            {
                interestValues[i] = Mathf.Clamp01(Vector3.Dot(targetDirection, rayDirections[i]));
                if (isOnScreen)
                {
                    if (Physics.Raycast(body.position, rayDirections[i], out RaycastHit info, obstacleDetectionRadius, ObstacleLayerMask))
                    {
                        if (Vector3.Dot(targetDirection, rayDirections[i]) <= 0)
                        {
                            obstacleValues[i] = 0f;
                        }
                        else
                        {
                            float temp = Vector3.Distance(info.point, body.position);
                            obstacleValues[i] = 1 - temp.Map(maxAvoidanceRadius, obstacleDetectionRadius);
                        }
                    }
                    else
                    {
                        obstacleValues[i] = 0f;
                    }
                    Debug.DrawLine(body.position + Vector3.down, body.position + Vector3.down +
                        (rayDirections[i] * obstacleValues[i]), Color.red);
                }
                Debug.DrawLine(body.position, body.position +
                    (rayDirections[i] * interestValues[i]), Color.green);
            }
            Vector3 finalDirection = Vector3.zero;
            for (int i = 0; i < rayDirections.Length; i++)
            {
                finalDirection += (interestValues[i] * rayDirections[i]);
                finalDirection -= (obstacleValues[i] * rayDirections[i]);
            }
            finalDirection /= rayDirections.Length;
            finalDirection.Normalize();
            Debug.DrawLine(body.position, body.position + (finalDirection * 3), Color.magenta);
            return finalDirection;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawWireArc(body.position - (normal * maxAvoidanceRadius / 2), normal, Vector3.up, 360, maxAvoidanceRadius);
            Handles.color = Color.yellow;
            Handles.DrawWireArc(body.position - (normal * obstacleDetectionRadius / 2), normal, Vector3.up, 360, obstacleDetectionRadius);
        }
#endif
    }
}
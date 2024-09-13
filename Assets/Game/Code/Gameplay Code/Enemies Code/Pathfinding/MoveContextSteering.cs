using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    [System.Serializable]
    public class MoveContextSteering : BasePathfinding
    {
        [field: Header("PATHFINDING PARAMETERS"), HorizontalLine(2f, EColor.Orange)]
        [field: SerializeField] public LayerMask ObstacleLayerMask { get; private set; }
        [SerializeField, Range(8,64)] int numberOfRays = 16;
        [SerializeField] float obstacleDetectionRadius = 2;
        [SerializeField] float maxAvoidanceRadius = 0.75f;


        [Header("PATHFINDING VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, AllowNesting, ReadOnly] Vector3 normal = new Vector3(0, 1, -1);
        Vector3[] rayDirections;
        float[] interestValues;
        float[] obstacleValues;

        public override Vector3 GetMovementDirection(Vector3 targetPosition, bool IsInsidePlayZone)
        { 
            return CalculateContextSteering(CalculateTargetDirection(targetPosition), IsInsidePlayZone);
        }
        public virtual Vector3 CalculateTargetDirection(Vector3 targetPosition)
        {
            Vector3 targetDirection = targetPosition - body.position;
            targetDirection.Normalize();
            Helper.DrawDirArrow(body.position, targetDirection, Color.yellow, Color.green);
            return targetDirection;
        }

        private Vector3 CalculateContextSteering(Vector3 targetDirection, bool IsInsidePlayZone)
        {
            for (int i = 0; i < rayDirections.Length; i++)
            {
                interestValues[i] = 0f;
                obstacleValues[i] = 0f;
                interestValues[i] = Mathf.Clamp01(Vector3.Dot(targetDirection, rayDirections[i]));
                if (IsInsidePlayZone)
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
#if UNITY_EDITOR
                    Debug.DrawLine(body.position + Vector3.down, body.position + Vector3.down +
                        (rayDirections[i] * obstacleValues[i]), Color.red);
#endif
                }
#if UNITY_EDITOR
                Debug.DrawLine(body.position, body.position +
                    (rayDirections[i] * interestValues[i]), Color.green);
#endif
            }
            Vector3 finalDirection = Vector3.zero;
            for (int i = 0; i < rayDirections.Length; i++)
            {
                finalDirection += (interestValues[i] * rayDirections[i]);
                finalDirection -= (obstacleValues[i] * rayDirections[i]);
            }
            finalDirection /= rayDirections.Length;
            finalDirection.Normalize();
#if UNITY_EDITOR
            Helper.DrawDirArrow(body.position, finalDirection * 2, Color.blue, Color.magenta, 0.5f);
#endif
            return finalDirection;
        }

        public override void Setup()
        {
            rayDirections = new Vector3[numberOfRays];
            interestValues = new float[numberOfRays];
            obstacleValues = new float[numberOfRays];
            for (int i = 0; i < numberOfRays; i++)
            {
                float angle = 2 * Mathf.PI / numberOfRays * i;
                angle *= 180 / Mathf.PI;
                rayDirections[i] = Quaternion.Euler(0, 0, angle) * Vector3.up;
                rayDirections[i] = new Vector3(rayDirections[i].x, rayDirections[i].y, rayDirections[i].y);
                rayDirections[i].Normalize();
            }
        }

        public override void OnGizmo()
        {
#if UNITY_EDITOR
            if(body == null)
            {
                return;
            }
            Handles.color = Color.red;
            Handles.DrawWireArc(body.position - (normal * maxAvoidanceRadius / 2), normal, Vector3.up, 360, maxAvoidanceRadius);
            Handles.color = Color.yellow;
            Handles.DrawWireArc(body.position - (normal * obstacleDetectionRadius / 2), normal, Vector3.up, 360, obstacleDetectionRadius);
#endif
        }

    }
}
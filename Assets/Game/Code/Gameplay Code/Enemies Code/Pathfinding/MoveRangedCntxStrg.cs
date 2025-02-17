using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    /// <summary>
    /// Moves the enemy using context steering behaviour with ranged implementation.
    /// </summary>
    [System.Serializable]
    public class MoveRangedCntxStrg : BasePathfinding
	{
        [Header("PATHFINDING REFERENCES"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] EnemyStateMachine stateMachine;

        [Header("PATHFINDING PARAMETERS"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, Range(8, 64)] int numberOfRays = 16;
        [Space]
        [SerializeField] float distanceToKeep = 11;
        [SerializeField] float distanceRange = 4;
        [field:Space]
        [field: SerializeField] public LayerMask ObstacleLayerMask { get; private set; }
        [SerializeField] float obstacleDetectionRadius = 2;
        [SerializeField] float maxAvoidanceRadius = 0.75f;

        [Header("PATHFINDING VARIABLES"), HorizontalLine(2f, EColor.Green)]
#pragma warning disable
        [SerializeField,AllowNesting, ReadOnly] string state;
#pragma warning enable
        [SerializeField, AllowNesting, ReadOnly] float distance;
        [SerializeField, AllowNesting, ReadOnly] Vector3 direction;
        [SerializeField, AllowNesting, ReadOnly] Vector3 normal = new Vector3(0, 1, -1);
        Vector3[] rayDirections;
        float[] interestValues;
        float[] obstacleValues;
        List<Vector3> obstaclePoints = new();

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
            if (body == null)
            {
                return;
            }
            Handles.color = Color.red;
            Handles.DrawWireArc(body.position - (normal * maxAvoidanceRadius / 2), normal, Vector3.up, 360, maxAvoidanceRadius);
            Handles.color = Color.yellow;
            Handles.DrawWireArc(body.position - (normal * obstacleDetectionRadius / 2), normal, Vector3.up, 360, obstacleDetectionRadius);
            Gizmos.color = Color.red;
            foreach(var point in obstaclePoints)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
#endif
        }

        public override Vector3 GetMovementDirection(Vector3 targetPosition, bool IsInsidePlayZone)
        {
            return CalculateContextSteering(CalculateTargetDirection(targetPosition), IsInsidePlayZone);
        }
        public Vector3 CalculateTargetDirection(Vector3 targetPosition)
        {
            Vector3 targetDirection = (targetPosition - body.position).normalized;

            distance = Vector3.Distance(targetPosition, body.position);
            if (distance > distanceToKeep || !stateMachine.IsOnScreen)
            {
                state = "Moving towards";
                direction = targetDirection;
                return direction;
            }

            bool obstacleBackward = Physics.Raycast(body.position, -targetDirection, 2f, ObstacleLayerMask);

            if (distance < distanceToKeep - distanceRange && !obstacleBackward)
            {
                state = "Moving away";

                direction = -targetDirection;
                return direction;
            }

            state = "Lining Up";
            direction = (new Vector3(body.position.x, targetPosition.z, targetPosition.z) - body.position).normalized;          
            return direction;
        }

        private Vector3 CalculateContextSteering(Vector3 targetDirection, bool IsInsidePlayZone)
        {
            obstaclePoints.Clear();
            for (int i = 0; i < rayDirections.Length; i++)
            {
                interestValues[i] = 0f;
                obstacleValues[i] = 0f;
                interestValues[i] = Mathf.Clamp01(Vector3.Dot(targetDirection, rayDirections[i]));
                if (IsInsidePlayZone)
                {
                    if (Physics.Raycast(body.position, rayDirections[i], out RaycastHit info, obstacleDetectionRadius, ObstacleLayerMask))
                    {
                        obstaclePoints.Add(info.point);
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
    }
}
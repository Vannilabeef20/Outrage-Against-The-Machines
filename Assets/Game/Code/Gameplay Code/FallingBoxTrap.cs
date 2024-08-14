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
    /// Creates a zone that spawns falling boxes.
    /// </summary>
    [SelectionBase]
    public class FallingBoxTrap : MonoBehaviour
    {
        [field: Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [Tooltip("How fast the spawned boxes will fall.")]
        [SerializeField, Expandable] FallingBoxSpeedSO boxFallingSpeed;
        [Tooltip("The prefab for the falling box.")]
        [SerializeField] GameObject fallingBoxObject;

        [field: Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]

        [Tooltip("How close the camera has to spawn boxes.")]
        [SerializeField] float activationRange;

        [Tooltip("How further in the X and YZ axis the zone extends from the Transform.")]
        [SerializeField] Vector2 spawnZoneDimensions;

        [Tooltip("How often (seconds) the trap will spawn boxes.")]
        [SerializeField, Range(0.1f, 5)] float spawnInterval = 0.5f;

        [Space]

        [Tooltip("Used exclusively by the Set Height Button\n" +
            "Tells the set height button how long the boxes have to fall for.")]
        [SerializeField] float targetFallTime;

        [field: Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [Tooltip("The approximated time the spawned boxes will take to fall.")]
        [SerializeField, ReadOnly] float fallTime;

        [Tooltip("How many seconds have passed since the last box was spawned.")]
        [SerializeField, ReadOnly] float spawnTimer;

        private void Update()
        {
            if (Mathf.Abs(GameManager.Instance.MainCamera.transform.position.x -
                transform.position.x) > activationRange) //Return if not within the activation range
            {
                return;
            }
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnInterval)
            {
                Spawn();
            }
        }
        /// <summary>
        /// Spawns a box within the spawn zone.
        /// </summary>
        public void Spawn()
        {
            spawnTimer = 0;
            float depth = Random.Range(-spawnZoneDimensions.y, spawnZoneDimensions.y);
            Vector3 randomPos = Vector3.zero;
            randomPos.x = transform.position.x + Random.Range(-spawnZoneDimensions.x, spawnZoneDimensions.x);
            randomPos.y = transform.position.y + depth;
            randomPos.z = transform.position.z + depth;
            Instantiate(fallingBoxObject, randomPos, Quaternion.identity, transform);
        }

        /// <summary>
        /// Sets trap height based on targetFallTime.
        /// </summary>
        [Button("Set Height")]
        public void SetHeight()
        {
            transform.position = new Vector3(transform.position.x, (targetFallTime * boxFallingSpeed.FallingSpeed) + transform.position.z, transform.position.z);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            #region DRAW SPAWN/DANGER ZONE
            Vector3 left;
            Vector3 right;
            left = new Vector3(transform.position.x + spawnZoneDimensions.x, 0f, 0f);
            right = new Vector3(transform.position.x - spawnZoneDimensions.x, 0f, 0f);
            Vector3 maxDephtUpperHeight;
            Vector3 minDephtUpperHeight;
            Vector3 maxDephtLowerHeight;
            Vector3 minDephtLowerHeight;
            maxDephtUpperHeight = new Vector3(0f, transform.position.y + spawnZoneDimensions.y, transform.position.z + spawnZoneDimensions.y);
            minDephtUpperHeight = new Vector3(0f, transform.position.y - spawnZoneDimensions.y, transform.position.z - spawnZoneDimensions.y);
            maxDephtLowerHeight = new Vector3(0f, transform.position.z + spawnZoneDimensions.y, transform.position.z + spawnZoneDimensions.y);
            minDephtLowerHeight = new Vector3(0f, transform.position.z - spawnZoneDimensions.y, transform.position.z - spawnZoneDimensions.y);

            Debug.DrawLine(left + maxDephtUpperHeight, right + maxDephtUpperHeight, Color.red); // Upper back zone line

            Debug.DrawLine(left + minDephtUpperHeight, right + minDephtUpperHeight, Color.red); // Upper front zone line

            Debug.DrawLine(left + maxDephtUpperHeight, left + minDephtUpperHeight, Color.red); // Upper left zone line

            Debug.DrawLine(right + maxDephtUpperHeight, right + minDephtUpperHeight, Color.red); // Upper right zone line


            Debug.DrawLine(left + maxDephtLowerHeight, right + maxDephtLowerHeight, Color.red); // Lower back zone line

            Debug.DrawLine(left + minDephtLowerHeight, right + minDephtLowerHeight, Color.red); // Lower front zone line

            Debug.DrawLine(left + maxDephtLowerHeight, left + minDephtLowerHeight, Color.red); // Lower left zone line

            Debug.DrawLine(right + maxDephtLowerHeight, right + minDephtLowerHeight, Color.red); // Lower right zone line
            #endregion

            Helper.DrawPointArrow(transform.position, transform.position.ToXZZ(), //Arrow pointing from spawn to ground
                Color.yellow, Color.red);
            if (boxFallingSpeed != null)
            {
                fallTime = (transform.position.y - transform.position.z) / boxFallingSpeed.FallingSpeed;
            }

            #region DRAW ACTIVATION RANGE ZONE
            Vector3 point1Pos = Vector3.zero;
            Vector3 point2Pos = Vector3.zero;

            point1Pos.x = transform.position.x - activationRange;
            point2Pos.x = transform.position.x - activationRange;
            point1Pos.y = transform.position.y;
            point2Pos.y = transform.position.z - spawnZoneDimensions.y;
            point1Pos.z = transform.position.z;
            point2Pos.z = transform.position.z;
            Debug.DrawLine(point1Pos, point2Pos, Color.green); //Activation range Left Line


            point1Pos.x = transform.position.x + activationRange;
            point2Pos.x = transform.position.x + activationRange;
            point1Pos.y = transform.position.y;
            point2Pos.y = transform.position.z - spawnZoneDimensions.y;
            point1Pos.z = transform.position.z;
            point2Pos.z = transform.position.z;
            Debug.DrawLine(point1Pos, point2Pos, Color.green); //Activation range RightLine
            #endregion
        }
#endif
    }
}

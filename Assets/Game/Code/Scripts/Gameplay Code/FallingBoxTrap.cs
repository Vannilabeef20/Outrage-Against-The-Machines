using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class FallingBoxTrap : MonoBehaviour
    {
        [SerializeField] private Vector2 spawnZoneDimensions;
        [SerializeField] private GameObject fallingBoxObject;
        [SerializeField, Range(0.5f, 5)] private float spawnInterval = 0.5f;
        [SerializeField, ReadOnly] private float spawnTimer;
        [SerializeField] private float activationRange;
        [SerializeField, Expandable] private FallingBoxSpeedSO boxFallingSpeed;
        [SerializeField, ReadOnly] private float fallTime;

        private void Update()
        {
            if((Mathf.Abs(GameManager.Instance.MainCamera.transform.position.x - 
                transform.position.x) > activationRange))
            {
                return;
            }
            spawnTimer += Time.deltaTime;
            if(spawnTimer > spawnInterval)
            {
                Spawn();
            }
        }
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
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 left = new Vector3(transform.position.x + spawnZoneDimensions.x, 0f, 0f);
            Vector3 right = new Vector3(transform.position.x - spawnZoneDimensions.x, 0f, 0f);
            Vector3 maxDephtUpperHeight = new Vector3(0f, transform.position.y + spawnZoneDimensions.y, transform.position.z + spawnZoneDimensions.y);
            Vector3 minDephtUpperHeight = new Vector3(0f, transform.position.y - spawnZoneDimensions.y, transform.position.z - spawnZoneDimensions.y);
            Vector3 maxDephtLowerHeight = new Vector3(0f, transform.position.z + spawnZoneDimensions.y, transform.position.z + spawnZoneDimensions.y);
            Vector3 minDephtLowerHeight = new Vector3(0f, transform.position.z - spawnZoneDimensions.y, transform.position.z - spawnZoneDimensions.y);

            Debug.DrawLine(left + maxDephtUpperHeight, right + maxDephtUpperHeight, Color.red); // Upper back line

            Debug.DrawLine(left + minDephtUpperHeight, right + minDephtUpperHeight, Color.red); // Upper front line

            Debug.DrawLine(left + maxDephtUpperHeight, left + minDephtUpperHeight, Color.red); // Upper left line

            Debug.DrawLine(right + maxDephtUpperHeight, right + minDephtUpperHeight, Color.red); // Upper right line


            Debug.DrawLine(left + maxDephtLowerHeight, right + maxDephtLowerHeight, Color.red); // Lower back line

            Debug.DrawLine(left + minDephtLowerHeight, right + minDephtLowerHeight, Color.red); // Lower front line

            Debug.DrawLine(left + maxDephtLowerHeight, left + minDephtLowerHeight, Color.red); // Lower left line

            Debug.DrawLine(right + maxDephtLowerHeight, right + minDephtLowerHeight, Color.red); // Lower right line

            Helper.DrawPointArrow(transform.position, transform.position.ToZ2D(),
                Color.yellow, Color.red);
            if(boxFallingSpeed != null)
            {
                fallTime = (transform.position.y - transform.position.z) / boxFallingSpeed.FallingSpeed;
            }
            Vector3 point1Pos = Vector3.zero;
            Vector3 point2Pos = Vector3.zero;

            point1Pos.x = transform.position.x - activationRange;
            point2Pos.x = transform.position.x - activationRange;
            point1Pos.y = transform.position.y;
            point2Pos.y = transform.position.z;
            point1Pos.z = transform.position.z;
            point2Pos.z = transform.position.z;
            Debug.DrawLine(point1Pos, point2Pos, Color.yellow);


            point1Pos.x = transform.position.x + activationRange;
            point2Pos.x = transform.position.x + activationRange;
            point1Pos.y = transform.position.y;
            point2Pos.y = transform.position.z;
            point1Pos.z = transform.position.z;
            point2Pos.z = transform.position.z;
            Debug.DrawLine(point1Pos, point2Pos, Color.yellow);
        }
#endif
    } 
}

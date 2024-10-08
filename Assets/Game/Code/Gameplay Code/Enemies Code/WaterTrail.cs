using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class WaterTrail : MonoBehaviour
	{
        [SerializeField] GameObject startTrail;
        [SerializeField] GameObject endTrail;
        [SerializeField] GameObject[] middleTrail;
        [SerializeField] int trailPixelLenght;
        [SerializeField] int trailLenght;
        [SerializeField] float placeInterval = 0.1f;
        [SerializeField] Vector3 offset;
        [SerializeField] float destroyTime = 2;

        const int PPU = 24;
        int TrailSpriteLenght => PPU / trailPixelLenght;

        public void CreateTrail()
        {
            StartCoroutine(BuildTrail());
        }
        public void StopTrail()
        {
            StopCoroutine(BuildTrail());
        }
        IEnumerator BuildTrail()
        {
            GameObject toBeSpawned;
            Vector3 initialPos = transform.position;
            Vector3 spawnPos = transform.position;
            Quaternion initialRot = transform.rotation;

            int dirMultiplier = 1;
            if (transform.rotation.eulerAngles.y >= 180) dirMultiplier = -1;

            for (int i = 0; i < trailLenght; i++)
            {
                if (i == 0) toBeSpawned = startTrail;
                else if (i == trailLenght - 1) toBeSpawned = endTrail;
                else toBeSpawned = middleTrail[UnityEngine.Random.Range(0, middleTrail.Length)];

                spawnPos = initialPos;
                spawnPos.x += dirMultiplier * (offset.x + (i * TrailSpriteLenght));
                spawnPos.y += offset.y;
                spawnPos.z += offset.z;

                GameObject spawnedPiece = Instantiate(toBeSpawned, spawnPos, initialRot);
                Destroy(spawnedPiece, destroyTime);
                yield return new WaitForSeconds(placeInterval);
            }
        }

#if UNITY_EDITOR

        [Button]
        void TestTrail()
        {
            StartCoroutine(BuildTrail());
        }
        private void OnDrawGizmos()
        {
            Vector3 finalPos = offset + (Vector3.right * (trailLenght * TrailSpriteLenght));
            Helper.DrawPointArrow(transform.position, finalPos, Color.yellow, Color.red);
            Debug.DrawLine(transform.position, finalPos, Color.gray);
        }
#endif
    }
}
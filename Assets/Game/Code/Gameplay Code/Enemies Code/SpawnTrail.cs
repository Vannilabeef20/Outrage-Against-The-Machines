using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class SpawnTrail : MonoBehaviour
	{
        [SerializeField] GameObject startTrail;
        [SerializeField] GameObject endTrail;
        [SerializeField] GameObject[] middleTrail;
        [SerializeField] float interval;
        [SerializeField] int trailLenght;
        [SerializeField] float placeInterval = 0.1f;
        [SerializeField] Vector3 offset;

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
            Vector3 spawnPos = transform.position;
            Vector3 initialPos = transform.position;
            Quaternion initialRot = transform.rotation;

            int dirMultiplier = 1;
            if (transform.rotation.eulerAngles.y >= 180) dirMultiplier = -1;

            for (int i = 0; i < trailLenght; i++)
            {
                if (i == 0) toBeSpawned = startTrail;
                else if (i == trailLenght - 1) toBeSpawned = endTrail;
                else toBeSpawned = middleTrail[UnityEngine.Random.Range(0, middleTrail.Length)];

                spawnPos = initialPos + offset;
                spawnPos.x += dirMultiplier * i * interval;               

                Instantiate(toBeSpawned, spawnPos, initialRot);
                yield return new WaitForSeconds(placeInterval);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 finalPos = transform.position + offset + (Vector3.right * (trailLenght * interval));
            Helper.DrawPointArrow(transform.position, finalPos, Color.cyan, Color.cyan);
        }
#endif
    }
}
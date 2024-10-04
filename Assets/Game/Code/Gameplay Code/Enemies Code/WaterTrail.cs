using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class WaterTrail : MonoBehaviour
	{
        [SerializeField] EnemyStateMachine enemyStateMachine;
        [SerializeField] GameObject startTrail;
        [SerializeField] GameObject endTrail;
        [SerializeField] GameObject[] middleTrail;
        [SerializeField] int trailPixelLenght;
        [SerializeField] int minLenght = 5;
        [SerializeField] float placeInterval = 0.1f;

        const int PPU = 24;
        int TrailSpriteLenght => PPU / trailPixelLenght;

        public void CreateTrail()
        {
            StartCoroutine(BuildTrail((int)enemyStateMachine.Distance));
        }
        IEnumerator BuildTrail(int targetDistance)
        {
            if(targetDistance < minLenght) targetDistance = minLenght;


            int trailSpriteNumber = Mathf.Abs(targetDistance) / TrailSpriteLenght;

            GameObject toBeSpawned;
            Vector3 localPoint = Vector3.zero;
            for (int i = 0; i < trailSpriteNumber; i++)
            {
                if (i == 0) toBeSpawned = startTrail;
                else if (i == trailSpriteNumber - 1) toBeSpawned = endTrail;
                else toBeSpawned = middleTrail[UnityEngine.Random.Range(0, middleTrail.Length)];

                localPoint.x += TrailSpriteLenght;

                Destroy(Instantiate(toBeSpawned, transform.TransformPoint(localPoint), transform.rotation), 5f);
                yield return new WaitForSeconds(placeInterval);
            }
        }

#if UNITY_EDITOR
        [SerializeField] float testDistance = 5f;

        [Button]
        void TestTrail()
        {
            StartCoroutine(BuildTrail((int)transform.TransformPoint(testDistance * Vector3.right).x));
        }
        private void OnDrawGizmos()
        {
            Vector3 finalPos;
            if (testDistance < minLenght)
                finalPos = transform.TransformPoint(minLenght * Vector3.right);
            else
                finalPos = transform.TransformPoint(testDistance * Vector3.right);

            Helper.DrawPointArrow(transform.position, finalPos, Color.yellow, Color.red);
            Debug.DrawLine(transform.position, finalPos, Color.gray);
        }
#endif
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public class ScrapDropper : MonoBehaviour
    {
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] Transform referencePoint;
        [SerializeField] StudioEventEmitter dropEmitter;
        [Space]
        [SerializeField, ShowAssetPreview] GameObject scrapUnit;
        [SerializeField, ShowAssetPreview] GameObject scrapTen;
        [SerializeField, ShowAssetPreview] GameObject scrapHundred;

        [Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField, Range(0, 90)] float rotAngle = 45f;
        [SerializeField, Min(0)] int minScapDrop;
        [SerializeField, Min(0)] int maxScapDrop;
        [SerializeField] bool hasReserveScrap;
        [SerializeField, ShowIf("hasReserveScrap")] int reserveAmount;

        public const int scrapTier1Value = 1;
        public const int scrapTier2Value = 5;
        public const int scrapTier3Value = 10;


        [Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, ReadOnly] int randomAmount;
        [SerializeField, ReadOnly] List<GameObject> scrapList = new();

        private void Awake()
        {
            randomAmount = UnityEngine.Random.Range(minScapDrop, maxScapDrop + 1);

            // Decompose drop amount in hundreds tens and units
            int hundreds = (int)(randomAmount / 10f);
            int tempRemainder = randomAmount % 10;

            int tens = (int)(tempRemainder / 5f);
            tempRemainder %= 5;

            int units = tempRemainder;

            // Populate a list with the correct amount of each scrap type
            for (int i = 0; i < hundreds; i++)
            {
                scrapList.Add(scrapHundred);
            }
            for (int i = 0; i < tens; i++)
            {
                scrapList.Add(scrapTen);
            }
            for (int i = 0; i < units; i++)
            {
                scrapList.Add(scrapUnit);
            }
        }
        public void SpawnAllScrap()
        {
            if (scrapList.Count <= 0) return;
            // Get the angle that the impulse will be applied to the scraps
            Vector3 dir;
            for (int i = 0; i < scrapList.Count; i++)
            {
                float angle = 2 * Mathf.PI / scrapList.Count * i;
                angle *= 180 / Mathf.PI;
                dir = Quaternion.AngleAxis(angle, referencePoint.up) * referencePoint.right;
                dir.Normalize();
                float spread = rotAngle.Map(0, 90, 0, 1);
                dir = Vector3.Slerp(dir, referencePoint.up, spread);
                dir.Normalize();
                ScrapDrop scrapDrop = Instantiate(scrapList[i], transform.position,
                    Quaternion.identity).GetComponentInChildren<ScrapDrop>();
                scrapDrop.ApplyForce(dir);
            }

            dropEmitter.Play();
        }

        public void SpawnRandomScrap()
        {
            //Return if theres nothing to spawn
            if (scrapList.Count <= 0 && !hasReserveScrap) return;

            //Return if theres nothing but the reserve to spawn
            if (scrapList.Count <= reserveAmount && hasReserveScrap) return;

            // Get the angle and the impulse will be applied to the scrap
            Vector3 dir;
            int randomIndex;

            randomIndex = UnityEngine.Random.Range(0, scrapList.Count);
            float angle = 2 * Mathf.PI / scrapList.Count * randomIndex;
            angle *= 180 / Mathf.PI;
            dir = Quaternion.AngleAxis(angle, referencePoint.up) * referencePoint.right;
            dir.Normalize();
            float spread = rotAngle.Map(0, 90, 0, 1);
            dir = Vector3.Slerp(dir, referencePoint.up, spread);
            dir.Normalize();
            ScrapDrop scrapDrop = Instantiate(scrapList[randomIndex], transform.position,
                Quaternion.identity).GetComponentInChildren<ScrapDrop>();
            scrapDrop.ApplyForce(dir);

            scrapList.RemoveAt(randomIndex);

            dropEmitter.Play();
        }
#if UNITY_EDITOR
        [Button("Test Spawn All")]
        void TestSpawnRandomScrapAmount()
        {
            // Get the angle that the impulse will be applied to the scraps
            Vector3 dir;
            float angle;
            for (int i = 0; i < scrapList.Count; i++)
            {
                angle = 2 * Mathf.PI / scrapList.Count * i;
                angle *= 180 / Mathf.PI;
                dir = Quaternion.AngleAxis(angle, referencePoint.up) * referencePoint.right;
                dir.Normalize();
                float spread = rotAngle.Map(0, 90, 0, 1);
                dir = Vector3.Slerp(dir, referencePoint.up, spread);
                dir.Normalize();
                Debug.DrawRay(referencePoint.position, dir, Color.yellow, 10f);
            }
        }
#endif
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class ScrapDropper : MonoBehaviour
    {
        [Header("Scrap"), HorizontalLine(2f, EColor.Red)]
        [SerializeField, ShowAssetPreview] GameObject scrapUnit;
        [SerializeField, ShowAssetPreview] GameObject scrapTen;
        [SerializeField, ShowAssetPreview] GameObject scrapHundred;
        [SerializeField, Min(0)] int minScapDrop;
        [SerializeField, Min(0)] int maxScapDrop;

        public void SpawnRandomScrapAmount()
        {
            List<GameObject> scrapList = new();
            // Randomly pick how much amount of scrap to drop
            int scrapAmount = UnityEngine.Random.Range(minScapDrop, maxScapDrop + 1);

            // Divide drop amount in hundreds tens and units
            int hundreds = (int)(scrapAmount / 100f);
            int tempRemainder = scrapAmount % 100;

            int tens = (int)(tempRemainder / 10f);
            tempRemainder %= 10;

            int units = tempRemainder;

            Debug.Log($"{scrapAmount} -> C{hundreds} D{tens} U{units}");

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

            // Get the angle that the impulse will be applied to the scraps
            Vector3[] directions = new Vector3[scrapList.Count];
            for (int i = 0; i < scrapList.Count; i++)
            {
                float angle = 2 * Mathf.PI / scrapList.Count * i;
                angle *= 180 / Mathf.PI;
                directions[i] = Quaternion.Euler(0, 0, angle) * Vector3.up;
                directions[i] = new Vector3(directions[i].x, directions[i].y, directions[i].y);
                directions[i].Normalize();
                ScrapDrop scrapDrop = Instantiate(scrapList[i], transform.position, Quaternion.identity).GetComponentInChildren<ScrapDrop>();
                Vector3 finalDirection = (directions[i] + Vector3.up).normalized;
                Debug.DrawRay(transform.position, finalDirection, Color.blue, 10f);
                scrapDrop.ApplyForce(finalDirection);
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class StoreInteractible : BaseInteractble
    {
        [Header("PURCHASE"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] GameObject item;
        [SerializeField] float coopPurchaseTime = 10f;
        [SerializeField] Color coopPurchaseColor;
        int originalCost;
        Color originalColor;

        private void Start()
        {
            originalCost = costAmount;
            originalColor = costTMP.color;
        }
        public override void Interact(int playerNumber)
        {
            if (!costsMoney)
            {
                OnInteract(playerNumber);
                return;
            }

            PlayerCharacter player = GameManager.Instance.PlayerCharacterList[playerNumber];

            if (player.scrapAmount >= costAmount)
            {
                player.scrapAmount -= costAmount;
                OnInteract(playerNumber);
                return;
            }

            if (player.scrapAmount <= 0) return;

            StartCoroutine(CoopPurchase(player));
        }

        IEnumerator CoopPurchase(PlayerCharacter player)
        {
            costTMP.color = coopPurchaseColor;
            int originalScrapAmount = player.scrapAmount;
            costAmount -= player.scrapAmount;
            player.scrapAmount = 0;
            UpdateCost();
            yield return new WaitForSeconds(coopPurchaseTime);
            costTMP.color = originalColor;
            player.scrapAmount = originalScrapAmount;
            costAmount += originalScrapAmount;
            UpdateCost();
            StopAllCoroutines();
        }

        public override void OnInteract(int playerNumber)
        {
            StopAllCoroutines();
            costTMP.color = originalColor;
            costAmount = originalCost;
            UpdateCost();
            Instantiate(item, transform.position.ToXYY(), Quaternion.identity);
        }
    }
}
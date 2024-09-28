using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using TMPro;

namespace Game
{
	public abstract class BaseInteractble : MonoBehaviour
	{
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] SpriteRenderer player1NumberRenderer;
        [SerializeField] SpriteRenderer player2NumberRenderer;
        [SerializeField] SpriteRenderer player3NumberRenderer;

        [Header("COST"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField] protected bool costsMoney;
        [SerializeField, ShowIf("costsMoney")] TextMeshPro costTMP;
        [SerializeField, Min(1), ShowIf("costsMoney")] protected int costAmount;

        private void Awake()
        {
            player1NumberRenderer.enabled = false;
            player2NumberRenderer.enabled = false;
            player3NumberRenderer.enabled = false;
            if (costsMoney) costTMP.text = $"${costAmount}";
            else costTMP.text = "";
        }

        public abstract void OnInteract(int playerNumber);
        public virtual void Interact(int playerNumber)
        {
            if (costsMoney)
            {
                //Has money
                if (GameManager.Instance.PlayerCharacterList[playerNumber].scrapAmount >= costAmount)
                {
                    GameManager.Instance.PlayerCharacterList[playerNumber].scrapAmount -= costAmount;
                    OnInteract(playerNumber);
                }
                else //Does not have money
                {
                    OnInteract(playerNumber);
                }
            }
            else 
            {
                OnInteract(playerNumber);
            }
        }

        public void UpdateSelection(int playerNumber, bool enable)
        {
            switch(playerNumber)
            {
                case 0: player1NumberRenderer.enabled = enable; break;
                case 1: player2NumberRenderer.enabled = enable; break;
                case 2: player3NumberRenderer.enabled = enable; break;
            }
        }
    }
}
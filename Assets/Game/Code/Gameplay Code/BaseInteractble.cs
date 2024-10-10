using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using TMPro;
using FMODUnity;

namespace Game
{
	public abstract class BaseInteractble : MonoBehaviour
	{
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] protected SpriteRenderer[] playerProximityRenderers;
        [SerializeField] protected StudioEventEmitter interactionEmitter;

        [Header("COST"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField] protected bool costsMoney;
        [SerializeField, ShowIf("costsMoney")] protected TextMeshPro costTMP;
        [SerializeField, Min(1), ShowIf("costsMoney")] protected int costAmount;

        void Awake()
        {
            foreach(var render in playerProximityRenderers)
            {
                render.enabled = false;
            }
            if (costsMoney) UpdateCost();
            else costTMP.text = "";
        }

        public abstract void OnInteract(int playerNumber);
        public virtual void Interact(int playerNumber)
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
        }

        protected void UpdateCost()
        {
            costTMP.text = $"${costAmount}";
        }

        public void UpdateSelection(int playerNumber, bool enable)
        {
            playerProximityRenderers[playerNumber].enabled = enable;
        }
    }
}
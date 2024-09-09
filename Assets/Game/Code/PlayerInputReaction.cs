using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Events;

namespace Game
{
	public class PlayerInputReaction : MonoBehaviour
	{
		[SerializeField] Button button;

		[SerializeField] EPlayerInput playerInput;

		[SerializeField] bool PlayerExclusive;

		[SerializeField, ShowIf("PlayerExclusive")] int playerIndex;



		public void React(PlayerGameInput playerGameInput)
        {

			if (playerInput != playerGameInput.Input) return;

			if (PlayerExclusive && playerIndex != playerGameInput.Index) return;

			button.onClick.Invoke();
		}
	}
}
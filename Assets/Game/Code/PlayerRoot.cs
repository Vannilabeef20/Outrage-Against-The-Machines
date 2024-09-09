using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game
{
	public class PlayerRoot : MonoBehaviour
	{
		[SerializeField] PlayerInput playerInput;

		[SerializeField] PlayerInputEvent inputEvent;


		public void ListenToInput(InputAction.CallbackContext context)
        {
			if (Time.timeScale <= 0) return;

			if (!context.performed) return;

			EPlayerInput input = (EPlayerInput)Enum.Parse(typeof(EPlayerInput), context.action.name);

			inputEvent.Raise(this, new PlayerGameInput((int)playerInput.user.index, input));
		}
	}
}
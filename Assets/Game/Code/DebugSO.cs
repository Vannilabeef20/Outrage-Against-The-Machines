using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Scriptable object that manages debug
    /// </summary>
	//[CreateAssetMenu(fileName = "DebugSO", menuName = "New DebugSO")]
	public class DebugSO : ScriptableObject
	{
        [field: Header("DEBUG MODE"), HorizontalLine(2f, EColor.Red)]
        [field: SerializeField] public bool IsDebugModeEnabled { get; private set; }

        [SerializeField] InputActionReference reference;

        [SerializeField, ReadOnly] string input_keys;


		[Header("DEBUG FILTERS"), HorizontalLine(2f, EColor.Orange)]
		[SerializeField] EDebugSubjectFlags DebugSubjects;
		[SerializeField] EDebugTypeFlags DebugTypes;

        private void OnEnable()
        {
            RefreshInputKeys();
			reference.action.performed += EnableDisableDebugMode;
            RefreshLoggerInfo();
        }

        private void OnDisable()
        {
            reference.action.performed -= EnableDisableDebugMode;
            reference.action.Reset();
            RefreshLoggerInfo();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RefreshInputKeys();
            RefreshLoggerInfo();
        }
#endif
        [Button("Refresh Input Keys")]
        private void RefreshInputKeys()
        {
            input_keys = default;
            for (int i = 0; i < reference.action.bindings.Count; i++)
            {
                if (i == 0)
                    input_keys = $"{reference.action.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontOmitDevice)}";
                else
                    input_keys += $",{reference.action.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontOmitDevice)}";

                if (i == reference.action.bindings.Count - 1) input_keys += ".";
            }
        }

        [Button("Test All CustomLogger Logs")]
        private void TestAllLogs()
        {
            this.Log("This is how the custom Log looks!");
            this.LogWarning("This is how the custom LogWarning looks!");
            this.LogError("This is how the custom LogError looks!");
        }

        private void EnableDisableDebugMode(InputAction.CallbackContext context)
		{
			if (!context.performed) return;

			IsDebugModeEnabled = !IsDebugModeEnabled;
            RefreshLoggerInfo();
        }
        
        void RefreshLoggerInfo()
        {
            CustomLogger.IsDebugModeEnabled = IsDebugModeEnabled;
            CustomLogger.DebugSubjects = DebugSubjects;
            CustomLogger.DebugTypes = DebugTypes;
        }
    }
}

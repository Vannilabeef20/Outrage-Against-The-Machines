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

        [Tooltip("Reference that defines the 'input_keys'.")]
        [SerializeField] InputActionReference reference;

        [Tooltip("Input paths for toggling the debug mode.")]
        [SerializeField, ReadOnly] string input_keys;


        [Header("DEBUG FILTERS"), HorizontalLine(2f, EColor.Orange)]

        [Tooltip("Enum flag filter, regards the subject of the debug operation.")]
        [SerializeField] EDebugSubjectFlags DebugSubjects;
        [Tooltip("Enum flag filter, regards the type of debug operation.")]
        [SerializeField] EDebugTypeFlags DebugTypes;

        private void Awake()
        {
            RefreshLoggerInfo();
            RefreshInputKeys();
        }
        private void OnEnable()
        {
            RefreshInputKeys();
            reference.action.performed += ToggleDebugMode;
            RefreshLoggerInfo();
        }

        private void OnDisable()
        {
            reference.action.performed -= ToggleDebugMode;
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
        /// <summary>
        /// Refreshes the "input_keys" string to match the input action reference.
        /// </summary>
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

        /// <summary>
        /// Logs all available kinds of logs in the "CustomLogger".
        /// </summary>
        [Button("Test All CustomLogger Logs")]
        private void TestAllLogs()
        {
            this.Log("This is how the custom Log looks!");
            this.LogWarning("This is how the custom LogWarning looks!");
            this.LogError("This is how the custom LogError looks!");
        }

        /// <summary>
        /// Toggles the debug mode on or off when the input action is performed.
        /// </summary>
        /// <param name="context"></param>
        private void ToggleDebugMode(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            IsDebugModeEnabled = !IsDebugModeEnabled;
            RefreshLoggerInfo();
        }

        /// <summary>
        /// Updates the customLogger to match this SO.
        /// </summary>
        void RefreshLoggerInfo()
        {
            CustomLogger.IsDebugModeEnabled = IsDebugModeEnabled;
            CustomLogger.DebugSubjects = DebugSubjects;
            CustomLogger.DebugTypes = DebugTypes;
        }
    }
}

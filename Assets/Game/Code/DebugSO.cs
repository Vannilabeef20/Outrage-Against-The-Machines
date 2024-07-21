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
	[CreateAssetMenu(fileName = "DebugSO", menuName = "New DebugSO")]
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
        }

        private void OnDisable()
        {
            reference.action.performed -= EnableDisableDebugMode;
            reference.action.Reset();
		}

#if UNITY_EDITOR
        private void OnValidate()
        {
            RefreshInputKeys();
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

        private void EnableDisableDebugMode(InputAction.CallbackContext context)
		{
			if (!context.performed) return;

			IsDebugModeEnabled = !IsDebugModeEnabled;
		}

        #region Debug Logs

        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="Sender">The object that sent the log request.</param>
        /// <param name="Message">The information to be displayed on the log.</param>
        /// <param name="EDebugSubject">EFlag for filtering.</param>
        public void Log(object Sender, object Message,
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Test)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.Log) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.Log($"<color=lime>{Message}</color>\n" +
                    $" <color=magenta>{EDebugSubject}</color><color=aqua> Sender: {Sender}</color>");
            }
        }

        /// <summary>
        /// Logs a warning message to the console.
        /// </summary>
        /// <param name="Sender">The object that sent the log request.</param>
        /// <param name="Message">The information to be displayed on the log.</param>
        /// <param name="EDebugSubject">EFlag for filtering.</param>
        public void LogWarning(object Sender, string Message,
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Test)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.LogWarning) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.LogWarning($"<color=yellow>{Message}</color>\n" +
                    $" <color=magenta>{EDebugSubject}</color><color=aqua> Sender: {Sender}</color>");
            }
        }

        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="Sender">The object that sent the log request.</param>
        /// <param name="Message">The information to be displayed on the log.</param>
        /// <param name="EDebugSubject">EFlag for filtering.</param>
        public void LogError(object Sender, string Message, 
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Test)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.LogError) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.LogError($"<color=red>{Message}</color>\n" +
                    $" <color=magenta>{EDebugSubject}</color><color=aqua> Sender: {Sender}</color>");
            }
        }

        [Button("Test All Logs")]
        private void TestAllLogs()
        {
            Log(this, "This is how the custom Log looks!", EDebugSubjectFlags.Test);
            LogWarning( this, "This is how the custom LogWarning looks!", EDebugSubjectFlags.Test);
            LogError(this, "This is how the custom LogError looks!", EDebugSubjectFlags.Test);
        }
        #endregion
    }

    /// <summary>
    /// EFlags for the subject of the debug operation.
    /// </summary>
    [System.Flags]
    public enum EDebugSubjectFlags
    {
        Test = 1,
        CustomEvents = 2,
    }

    /// <summary>
    /// EFlags for filtering the type of debug operation.
    /// </summary>
    [System.Flags]
    public enum EDebugTypeFlags
    {
        Log = 1,
        LogWarning = 2,
        LogError = 4,
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game
{
    [System.Serializable]
	[CreateAssetMenu(fileName = "DebugSO", menuName = "New DebugSO")]
	public class DebugSO : ScriptableObject
	{
        [field: Header("DEBUG MODE"), HorizontalLine(2f, EColor.Red)]
        [field: SerializeField] public bool IsDebugModeEnabled { get; private set; }

        [SerializeField] InputActionAsset playerInputActions;

        [SerializeField, ReadOnly] string activateDebugModeActionName = "DebugMode";
        [SerializeField, ReadOnly] InputAction debugModeAction;



		[Header("DEBUG FILTERS"), HorizontalLine(2f, EColor.Orange)]
		[SerializeField] EDebugSubjectFlags DebugSubjects;
		[SerializeField] EDebugTypeFlags DebugTypes;

        private void OnEnable()
        {
            debugModeAction = playerInputActions.FindAction(activateDebugModeActionName);
			debugModeAction.performed += EnableDisableDebugMode;
        }

        private void OnDisable()
        {
			debugModeAction.performed -= EnableDisableDebugMode;
		}

        private void EnableDisableDebugMode(InputAction.CallbackContext context)
		{
			if (!context.performed) return;

			IsDebugModeEnabled = !IsDebugModeEnabled;
		}

        public void Log(object Sender, string Message,
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Test)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.Log) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.Log($"<color=#AD85D6>{EDebugSubject}</color><color=aqua> {Sender}</color>: <color=lime>{Message}</color>");
            }
        }

        public void LogWarning(object Sender, string Message,
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Test)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.LogWarning) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.LogWarning($"<color=#AD85D6>{EDebugSubject}</color><color=aqua> {Sender}</color>: <color=yellow>{Message}</color>");
            }
        }

        public void LogError(object Sender, string Message, 
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Test)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.LogError) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.LogError($"<color=#AD85D6>{EDebugSubject}</color><color=aqua> {Sender}</color>: <color=red>{Message}</color>");
            }
        }

        [Button("Test All Logs")]
        public void TestAllLogs()
        {
            Log(this, "This is how the custom Log looks!", EDebugSubjectFlags.Test);
            LogWarning( this, "This is how the custom LogWarning looks!", EDebugSubjectFlags.Test);
            LogError(this, "This is how the custom LogError looks!", EDebugSubjectFlags.Test);
        }
    }

    public struct DebugLogStruct
    {
        public EDebugSubjectFlags EDebugSubject;
        public object Sender;
        public object Message;

        public DebugLogStruct(EDebugSubjectFlags DebugSubject,
            object Sender, object Message)
        {
            this.EDebugSubject = DebugSubject;
            this.Sender = Sender;
            this.Message = Message;
        }
    }

    [System.Flags]
    public enum EDebugSubjectFlags
    {
        Test = 1,
        Events = 2,
        PlayerInput = 4,
        PlayerMovement = 8,
        PlayerCombat = 16,
        PlayerHealth = 32,
        EnemyMovement = 64,
        UI = 128,
        UI_Events = 256,
    }


    [System.Flags]
    public enum EDebugTypeFlags
    {
        Log = 1,
        LogWarning = 2,
        LogError = 4,
    }
}

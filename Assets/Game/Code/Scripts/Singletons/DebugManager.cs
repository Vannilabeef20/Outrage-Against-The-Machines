using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager instance { get; private set; }

        [field:SerializeField, ReadOnly] public bool isDebugModeEnabled {get; private set;}

        [Header("DEBUG FILTERS"), HorizontalLine(2f, EColor.White)]
        [SerializeField] private EDebugSubjectFlags DebugSubjects;
        [SerializeField] private EDebugTypeFlags DebugTypes;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightAlt))
            {
                isDebugModeEnabled = !isDebugModeEnabled;
            }
        }

        public void Log(DebugLogStruct debugLogStruct)
        {
            if (((EDebugTypeFlags.Log & DebugTypes) != 0) && ((debugLogStruct.EDebugSubject & DebugSubjects) != 0))
            {
                Debug.Log($"<color=#AD85D6>{debugLogStruct.EDebugSubject}</color><color=aqua> {debugLogStruct.Sender}</color>: <color=lime>{debugLogStruct.Message}</color>");
            }
        }

        public void LogWarning(DebugLogStruct debugLogStruct)
        {
            if (((EDebugTypeFlags.LogWarning & DebugTypes) != 0) && ((debugLogStruct.EDebugSubject & DebugSubjects) != 0))
            {
                Debug.LogWarning($"<color=#AD85D6>{debugLogStruct.EDebugSubject}</color><color=aqua> {debugLogStruct.Sender}</color>: <color=yellow>{debugLogStruct.Message}</color>");
            }
        }

        public void LogError(DebugLogStruct debugLogStruct)
        {
            if (((EDebugTypeFlags.LogError & DebugTypes) != 0) && ((debugLogStruct.EDebugSubject & DebugSubjects) != 0))
            {
                Debug.LogError($"<color=#AD85D6>{debugLogStruct.EDebugSubject}</color><color=aqua> {debugLogStruct.Sender}</color>: <color=red>{debugLogStruct.Message}</color>");
            }
        }

        [Button("Test All Logs")]
        public void TestAllLogs()
        {
            Log(new DebugLogStruct
         (EDebugSubjectFlags.Test, this, "This is how the custom Log looks!"));
            LogWarning(new DebugLogStruct
         (EDebugSubjectFlags.Test, this, "This is how the custom LogWarning looks!"));
            LogError(new DebugLogStruct
         (EDebugSubjectFlags.Test, this, "This is how the custom LogError looks!"));
        }

        public bool Validate(EDebugTypeFlags debugType, EDebugSubjectFlags debugSubject)
        {
            if (((debugType & DebugTypes) != 0) && ((debugSubject & DebugSubjects) != 0))
            {
                return true;
            }
            else
            {
                return false;
            }
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

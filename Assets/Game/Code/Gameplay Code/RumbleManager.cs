using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class RumbleManager : MonoBehaviour
    {
        public static RumbleManager Instance { get; private set; }

        [field: Header("PARAMETERS"), HorizontalLine(2f, EColor.Red)]
        [field: SerializeField] public bool IsEnabled { get; private set; }
        [SerializeField, Min(0)] float updateInterval;

#if UNITY_EDITOR
        [field: Header("TESTING"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] EPlayer testTarget;
        [SerializeField] RumbleData testData;
#endif
        [field: Header("RUNTIME VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, ReadOnly] float timeScaleTimer;
        [SerializeField, ReadOnly] float realtimeTimer;
        [SerializeField] TotalPlayerRumble[] playersRumble;
        [SerializeField, ReadOnly] List<Rumble> rumbleList = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
                EditorApplication.playModeStateChanged += StopEditorRumble;
#endif
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            UpdateRumble();
        }

        private void OnLevelWasLoaded(int level)
        {
            //Actually should be on transition
            List<Rumble> timescaleRumbles = new();
            foreach(var rumble in rumbleList)
            {
                if(!rumble.IsRealtime) timescaleRumbles.Add(rumble);
            }
            foreach(var rumble in timescaleRumbles)
            {
                rumbleList.Remove(rumble);
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            foreach (TotalPlayerRumble playerRumble in playersRumble)
            {
                playerRumble.Name = $"{playerRumble.Target.ToString()}";
            }
        }
        void StopEditorRumble(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingPlayMode &&
                stateChange == PlayModeStateChange.EnteredEditMode) return;

            StopRumble();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Test()
        {
            CreateRumble("Test", testData, testTarget);
        }
#endif

        public void CreateRumble(string ownerName, RumbleData data, EPlayer target = EPlayer.All)
        {
            if (!IsEnabled) return;

            rumbleList.Add(new Rumble(ownerName, data.LowCurve, data.HighCurve, data.Duration, target, data.Realtime));          
        }

        public void CreateRumble(string ownerName, RumbleData data, int targetIndex = -1)
        {
            if (!IsEnabled) return;

            EPlayer target = (EPlayer)targetIndex;

            rumbleList.Add(new Rumble(ownerName, data.LowCurve, data.HighCurve, data.Duration, target, data.Realtime));
        }

        public void CancelRumble(string rumbleName)
        {
            List<Rumble> toRemove = new();
            foreach(var rumble in rumbleList)
            {
                if (rumble.Name != rumbleName) continue;

                toRemove.Add(rumble);
                break;
            }
            foreach(var rumble in toRemove)
            {
                rumbleList.Remove(rumble);
            }
        }

        public void StopRumble()
        {
            foreach (var gamepad in Gamepad.all)
            {
                gamepad.SetMotorSpeeds(0f, 0f);
            }
        }

        public void ToggleRumble(bool enabled)
        {
            if (enabled)
            {
                PlayerPrefs.GetInt("Rumble", 1);
            }
            else
            {
                PlayerPrefs.GetInt("Rumble", 0);
            }
            IsEnabled = enabled;
        }

        public void PauseStopRumble(bool paused)
        {
            foreach (Rumble runningRumble in rumbleList)
            {
                if (runningRumble.IsRealtime) return;

                runningRumble.isPaused = paused;
            }
        }


        public void UpdateRumble()
        {
            if (!IsEnabled) return;
            if (rumbleList.Count < 1) return;

            timeScaleTimer += Time.deltaTime;
            realtimeTimer += Time.unscaledDeltaTime;

            if (realtimeTimer < updateInterval) return;

            //Reset values
            foreach (TotalPlayerRumble playerRumble in playersRumble)
            {
                playerRumble.TotalLow = 0;
                playerRumble.TotalHigh = 0;
            }

            //Remove all finished Rumble
            List<Rumble> doneList = new List<Rumble>();
            foreach (Rumble runningRumble in rumbleList)
            {
                if (runningRumble.Done) doneList.Add(runningRumble);
            }
            foreach (Rumble doneRumble in doneList)
            {
                rumbleList.Remove(doneRumble);
            }

            //Sample all rumble and add to respective player total
            foreach (Rumble runningRumble in rumbleList)
            {
                if (runningRumble.isPaused) continue;

                if(runningRumble.IsRealtime)
                {
                    runningRumble.Sample(realtimeTimer);
                }
                else
                {
                    runningRumble.Sample(timeScaleTimer);
                }

                int targetIndex = (int)runningRumble.Target;

                if (runningRumble.Target == EPlayer.All || GameManager.Instance.PlayerCharacterList.Count == 1)
                {
                    //Add to all devices
                    foreach (TotalPlayerRumble player in playersRumble)
                    {
                        player.TotalLow += runningRumble.LowFreq;
                        player.TotalHigh += runningRumble.HighFreq;
                    }
                }
                else
                {
                    //Add to specific player device
                    playersRumble[targetIndex].TotalLow += runningRumble.LowFreq;
                    playersRumble[targetIndex].TotalHigh += runningRumble.HighFreq;
                }
            }

            //Execute rumble in each player
            foreach (TotalPlayerRumble playerRumble in playersRumble)
            {
                playerRumble.Clamp();
                if (playerRumble.Target == EPlayer.All)
                {
                    playerRumble.SetMotorSpeeds(InputSystem.devices.ToArray());
                }
                else
                {
                    int targetIndex = (int)playerRumble.Target;

                    if (targetIndex >= GameManager.Instance.
                        PlayerCharacterList.Count) continue;
                    playerRumble.SetMotorSpeeds(GameManager.Instance.
                        PlayerCharacterList[targetIndex].InitialDevices);
                }
            }

            //ResetTimers
            timeScaleTimer = 0;
            realtimeTimer = 0;
        }

        [Serializable]
        class TotalPlayerRumble
        {
            [HideInInspector] public string Name;
            [field: SerializeField] public EPlayer Target { get; private set; }

            [AllowNesting, ReadOnly] public float TotalLow;
            [AllowNesting, ReadOnly] public float TotalHigh;
            [AllowNesting, ReadOnly, Range(0f, 1f)] public float SpeedLow;
            [AllowNesting, ReadOnly, Range(0f, 1f)] public float SpeedHigh;

            public List<PlayerCharacter> Characters => GameManager.Instance.PlayerCharacterList;

            public void SetMotorSpeeds(InputDevice[] devices)
            {
                foreach (InputDevice device in devices)
                {
                    Gamepad gamepad;
                    try
                    {
                        gamepad = (Gamepad)device;
                    }
                    catch { continue; }

                    gamepad.SetMotorSpeeds(SpeedLow, SpeedHigh);
                }
            }

            public void Clamp()
            {
                SpeedLow = Mathf.Clamp(TotalLow, 0f, 1f);
                SpeedHigh = Mathf.Clamp(TotalHigh, 0f, 1f);
            }
        }
    }
    [Flags]
    public enum EPlayer
    {
        All = -1,
        Player_1,
        Player_2,
        Player_3,
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace Game
{
	public class RumbleManager : MonoBehaviour
	{
        public static RumbleManager Instance { get; private set; }

        [SerializeField] bool isRumbleEnabled;
        [SerializeField, Min(0)] float updateInterval;
        [SerializeField] RumbleData testData;
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

        public void UpdateRumble()
        {
            if (!isRumbleEnabled) return;
            if (rumbleList.Count <= 0) return;

            timeScaleTimer += Time.deltaTime;
            realtimeTimer += Time.unscaledDeltaTime;

            if (realtimeTimer < updateInterval) return;

            foreach(TotalPlayerRumble playerRumble in playersRumble)
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

            //Sample all rumble and add to respective player
            foreach (Rumble runningRumble in rumbleList)
            {
                runningRumble.Sample(timeScaleTimer);
                
                int targetIndex = (int)runningRumble.Target;


                if(targetIndex >= 0)
                {
                    //Specific
                    playersRumble[targetIndex].TotalLow += runningRumble.LowFreq;
                    playersRumble[targetIndex].TotalHigh += runningRumble.HighFreq;
                    continue;
                }
                //Add to all players
                foreach (var player in playersRumble)
                {
                    player.TotalLow += runningRumble.LowFreq;
                    player.TotalHigh += runningRumble.HighFreq;
                }
                //Add to nonPlayer devices

            }

            //Get all devices
            List<InputDevice> nonPlayerInputDevices = new();
            foreach(var device in InputSystem.devices)
            {
                nonPlayerInputDevices.Add(device);
            }

            //Take out player devices from nonPlayerDevices
            for(int i = 0; i < GameManager.Instance.PlayerCharacterList.Count; i++)
            {
                foreach(InputDevice device in GameManager.Instance.PlayerCharacterList[i].Devices)
                {
                    nonPlayerInputDevices.Remove(device);
                }
            }

            //Execute rumble in each player
            for(int i = 0; i < playersRumble.Length; i++)
            {
                playersRumble[i].Clamp();
                playersRumble[i].SetMotorSpeeds(i);
            }

            //ResetTimer
            timeScaleTimer = 0;
        }

        public void CreateRumble(string ownerName, RumbleData data)
        {
            rumbleList.Add(new Rumble(ownerName, data.LowCurve, data.HighCurve, data.Duration, data.Target));
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        void Test()
        {
            rumbleList.Add(new Rumble("Test", testData.LowCurve, testData.HighCurve, testData.Duration, testData.Target));
        }

        [Serializable]
        class TotalPlayerRumble
        {
            [AllowNesting, ReadOnly] public float TotalLow;
            [AllowNesting, ReadOnly] public float TotalHigh;
            [AllowNesting, ReadOnly, Range(0f,1f)] public float SpeedLow;
            [AllowNesting, ReadOnly, Range(0f,1f)] public float SpeedHigh;

            public List<PlayerCharacter> Characters => GameManager.Instance.PlayerCharacterList;

            public void SetMotorSpeeds(int index)
            {
                if(index >= Characters.Count) return;

                foreach (InputDevice device in Characters[index].Devices)
                {
                    Gamepad gamepad;
                    try
                    {
                        gamepad = (Gamepad)device;
                    }
                    catch { continue;}

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
}
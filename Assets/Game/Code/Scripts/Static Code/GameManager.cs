using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance{ get; private set; }

        [field: SerializeField, ReadOnly] public Camera MainCamera { private set; get; }

        #region Loading & Transitions Params
        [Header("Loading & Transitions"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField, ReadOnly] private Coroutine loadRoutine;

        [SerializeField] private Image transitionImage;

        [SerializeField] private TransitionSO transition;

        [SerializeField] private MenuIdEvent OnSetMenuVisibility;
        #endregion

        #region Players Params
        [field: Header("Players"), HorizontalLine(2f, EColor.Red)]

        public PlayerInputManager UnityInputManager;

        [field: SerializeField] public GameObject[] PlayerPrefabs { get; private set; }

        [field: SerializeField, ReadOnly] public List<PlayerCharacter> PlayerCharacterList { get; private set; }

        [SerializeField] private Vector3[] spawnCoordinates;

        [SerializeField] private GameObject followGroupPrefab;
        #endregion

        #region Lifes Params
        [SerializeField] private IntEvent UpdateLifeCount;
        [field: SerializeField, ReadOnly] public int CurrentLifeAmount { get; private set; }

        [SerializeField] private int initialLifeAmout;
        [SerializeField] private int maxLifeAmount;
        #endregion


        #region Unity/Application Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            transitionImage.enabled = false;
            int level = SceneManager.GetActiveScene().buildIndex;
            if (level != 0)
            {
                InitializeLevel();
            }
#if UNITY_EDITOR
            EditorApplication.quitting += StopRumble;
#endif
        }
        private void Start()
        {
            int level = SceneManager.GetActiveScene().buildIndex;
            if (level == 0)
            {
                OnSetMenuVisibility.Raise(this, MenuId.StartMenu);
            }
            else
            {
                CurrentLifeAmount = initialLifeAmout;
                UpdateLifeCount.Raise(this, CurrentLifeAmount);
                MainCamera = FindObjectOfType<Camera>();
                OnSetMenuVisibility.Raise(this, MenuId.None);               
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if(Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            if (level == 0)
            {
                OnSetMenuVisibility.Raise(this, MenuId.StartMenu);
            }
            else
            {
                OnSetMenuVisibility.Raise(this, MenuId.None);
                InitializeLevel();
            }           
            StartCoroutine(LoadOrTransitionRoutine());
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (Vector3 coordinate in spawnCoordinates)
            {
                Gizmos.DrawSphere(coordinate, 0.2f);
            }
        }
#endif
        private void OnApplicationQuit()
        {
            StopRumble();
        }
        #endregion

        private void InitializeLevel()
        {
            MainCamera = FindObjectOfType<Camera>();
            CurrentLifeAmount = initialLifeAmout;
            UpdateLifeCount.Raise(this, CurrentLifeAmount);
            if (PlayerCharacterList.Count == 0)
            {
                PlayerCharacterList.Add(new PlayerCharacter(PlayerPrefabs[0], 0, null, null));
                PlayerCharacterList[0].GameObject = Instantiate(PlayerPrefabs[0], spawnCoordinates[0], Quaternion.identity);
                PlayerCharacterList[0].isPlayerActive = true;
            }
            else
            {
                for (int i = 0; i < PlayerCharacterList.Count; i++)
                {
                    PlayerCharacterList[i].isPlayerActive = true;
                    PlayerCharacterList[i].GameObject = Instantiate(PlayerCharacterList[i].PlayerPrefab, spawnCoordinates[i], Quaternion.identity);
                }
            }
            Instantiate(followGroupPrefab);
        }

        #region Rumble Methods
        public void Rumble(InputDevice device, float lowFrequency, float highFrequency, float duration)
        {
            Gamepad gamepad;
            try
            {
                gamepad = (Gamepad)device;
            }
            catch
            {
                return;
            }

            StartCoroutine(PulseRumble(gamepad, lowFrequency, highFrequency, duration));
        }
        public void Rumble(float lowFrequency, float highFrequency, float duration)
        {   
            foreach(Gamepad gamepad in Gamepad.all)
            {
                StartCoroutine(PulseRumble(gamepad, lowFrequency, highFrequency, duration));
            }
        }

        private IEnumerator PulseRumble(Gamepad gamepad, float lowFrequency, float highFrequency, float duration)
        {
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            yield return new WaitForSecondsRealtime(duration);
            StopRumble(gamepad);
        }
        private IEnumerator PulseRumble(Gamepad[] gamepads, float lowFrequency, float highFrequency, float duration)
        {
            foreach(Gamepad gamepad in gamepads)
            {
                gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            }
            yield return new WaitForSecondsRealtime(duration);
            StopRumble();
        }
        private void StopRumble(Gamepad gamepad)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
        private void StopRumble()
        {
            foreach(var gamepad in Gamepad.all)
            {
                gamepad.SetMotorSpeeds(0f, 0f);
            }
        }
        #endregion

        public void PauseGame()
        {
            if(Time.timeScale > 0)
            {
                OnSetMenuVisibility.Raise(this, MenuId.PauseMenu);
            }
            else
            {
                OnSetMenuVisibility.Raise(this, MenuId.None);
            }
        }
        #region Loading Methods
        public void LoadScene(int targetSceneIndex)
        {
            if (loadRoutine == null)
            {
                loadRoutine = StartCoroutine(LoadOrTransitionRoutine(targetSceneIndex));
            }
        }
        
        private IEnumerator LoadOrTransitionRoutine(int sceneIndex = -1)
        {
            transitionImage.enabled = true;
            float frameTime = transition.Duration / transition.Sprites.Length;
            if (sceneIndex < 0) //Transition out of load
            {
                for (int i = transition.Sprites.Length - 1; i > 0; i--)
                {
                    transitionImage.sprite = transition.Sprites[i];
                    yield return new WaitForSecondsRealtime(frameTime);
                }
                transitionImage.enabled = false;
            }
            else //Transition in load
            {
                for (int i = 0; i < transition.Sprites.Length; i++)
                {
                    transitionImage.sprite = transition.Sprites[i];
                    yield return new WaitForSecondsRealtime(frameTime);
                }
                SceneManager.LoadScene(sceneIndex);
                loadRoutine = null;
            }                    
        }
        #endregion
        public Vector2 WorldToViewport2D(Vector3 worldPos)
        {
            return MainCamera.WorldToViewportPoint(worldPos);
        }
        public void TakeAddLife(int amount)
        {
            CurrentLifeAmount = Mathf.Clamp(CurrentLifeAmount + amount, 0, maxLifeAmount);
            for(int i = 0; i < PlayerCharacterList.Count; i++)
            {
                if (!PlayerCharacterList[i].isPlayerActive && CurrentLifeAmount > 0)
                {
                    PlayerCharacterList[i].GameObject.transform.position = new Vector3(MainCamera.transform.position.x,
                        MainCamera.transform.position.y - 1.5f, MainCamera.transform.position.y - 1.5f);
                    CurrentLifeAmount = Mathf.Clamp(CurrentLifeAmount--, 0, maxLifeAmount);
                    PlayerCharacterList[i].GameObject.SetActive(true);
                    PlayerCharacterList[i].GameObject.GetComponentInChildren<PlayerHealthHandler>().playerHitbox.enabled = true;
                    PlayerCharacterList[i].isPlayerActive = true;
                }
            }
            UpdateLifeCount.Raise(this, CurrentLifeAmount);
        }
        
        public void UpdatePlayerDeathStatus(PlayerDeathParams playerDeathParams)
        {
            PlayerCharacterList[playerDeathParams.playerID].isPlayerActive = !playerDeathParams.isPlayerDead;
            int aliveCount = 0;
            foreach(var character in PlayerCharacterList)
            {
                if(character.isPlayerActive)
                {
                    aliveCount++;
                }
            }
            if(aliveCount == 0)
            {
                LoadScene(0);
            }
        }

        #region Testing Methods
        [Button("Test start transition", EButtonEnableMode.Playmode)]
        public void TestRegularTransition()
        {
            StartCoroutine(LoadOrTransitionRoutine());
        }

        [Button("Test load transition", EButtonEnableMode.Playmode)]
        public void TestReverseTransition()
        {
            StartCoroutine(LoadOrTransitionRoutine(1));
        }
        #endregion
    }

    [Serializable]
    public class PlayerCharacter
    {
        [field: SerializeField] public GameObject PlayerPrefab { get; private set; }
        [field: SerializeField] public int PlayerIndex { get; private set; }
        [field: SerializeField] public string ControlScheme { get; private set; }
        [field: SerializeField] public InputDevice[] Devices { get; private set; }

        public bool isPlayerActive;

        public GameObject GameObject;

        public PlayerCharacter (GameObject playerPrefab, int playerIndex, string controlScheme, InputDevice[] devices)
        {
            PlayerPrefab = playerPrefab;
            PlayerIndex = playerIndex;
            ControlScheme = controlScheme;
            Devices = devices;
        }
    }
}


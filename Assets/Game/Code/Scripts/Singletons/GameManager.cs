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

        #region Loading & Transitions
        [Header("Loading & Transitions"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField, ReadOnly] private Coroutine loadRoutine;

        [SerializeField] private Image transitionImage;

        [SerializeField] private TransitionSO transition;

        [SerializeField] private MenuIdEvent OnSetMenuVisibility;



        #endregion

        #region Players
        [field: Header("Players"), HorizontalLine(2f, EColor.Red)]

        public PlayerInputManager UnityInputManager;

        [SerializeField] private GameObject[] playerCharacters;

        [SerializeField] private Vector3[] spawnCoordinates;

        public List<int> playerIndexes = new();

        public bool[] playerAlive = new bool[3];

        [field: SerializeField, ReadOnly] public GameObject[] PlayerObjectArray { private set; get; }

        [SerializeField] private GameObject followGroupPrefab;
        #endregion

        #region Lifes
        [SerializeField] private IntEvent UpdateLifeCount;
        [field: SerializeField, ReadOnly] public int CurrentLifeAmount { get; private set; }
        public int maxLifeAmount;
        #endregion

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
                Debug.Log("2");
            }           
            StartCoroutine(StartTransitionRoutine());
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
        public void LoadScene(int targetSceneIndex)
        {
            if (loadRoutine == null)
            {
                loadRoutine = StartCoroutine(SceneLoadRoutine(targetSceneIndex));
            }
        }

        private IEnumerator StartTransitionRoutine()
        {
            transitionImage.enabled = true;
            float frameTime = transition.Duration / transition.Sprites.Length;
            for (int i = transition.Sprites.Length - 1; i > 0; i--)
            {
                transitionImage.sprite = transition.Sprites[i];
                yield return new WaitForSecondsRealtime(frameTime);
            }
            transitionImage.enabled = false;
            loadRoutine = null;
        }

        private IEnumerator SceneLoadRoutine(int sceneIndex)
        {
            transitionImage.enabled = true;
            float frameTime = transition.Duration / transition.Sprites.Length;
            for (int i = 0; i < transition.Sprites.Length; i++)
            {
                transitionImage.sprite = transition.Sprites[i];
                yield return new WaitForSecondsRealtime(frameTime);
            }
            SceneManager.LoadScene(sceneIndex);
            loadRoutine = null;
        }      

        public Vector3 WorldToViewport3D(Vector3 worldPos)
        {
           return MainCamera.WorldToViewportPoint(worldPos);
        }
        public Vector2 WorldToViewport2D(Vector3 worldPos)
        {
            return MainCamera.WorldToViewportPoint(worldPos);
        }
        public void TakeAddLife(int amount)
        {
            CurrentLifeAmount += amount;
            UpdateLifeCount.Raise(this, CurrentLifeAmount);
        }

        private void InitializeLevel()
        {
            CurrentLifeAmount = maxLifeAmount;
            UpdateLifeCount.Raise(this, CurrentLifeAmount);
            UpdateLifeCount.Raise(this, CurrentLifeAmount);
            List<GameObject> players = new();
            if(playerIndexes.Count == 0)
            {
                GameObject player = Instantiate(playerCharacters[0], spawnCoordinates[0], Quaternion.identity);
                playerIndexes.Add(0);
                players.Add(player);
                Debug.Log("a");
            }
            else
            {
                for (int i = 0; i <= playerIndexes.Count - 1; i++)
                {
                    playerAlive[i] = true;
                    players.Add(Instantiate(playerCharacters[i], spawnCoordinates[i], Quaternion.identity));
                    Debug.Log("b");
                }
            }          
            PlayerObjectArray = players.ToArray();
            Instantiate(followGroupPrefab);
        }

        #region Testing
        [Button("Test start transition", EButtonEnableMode.Playmode)]
        public void TestRegularTransition()
        {
            StartCoroutine(StartTransitionRoutine());
        }

        [Button("Test load transition", EButtonEnableMode.Playmode)]
        public void TestReverseTransition()
        {
            StartCoroutine(SceneLoadRoutine(1));
        }
        #endregion
    }
}


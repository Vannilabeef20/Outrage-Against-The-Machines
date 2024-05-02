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

        public static GameManager Instance;

        [field: SerializeField, ReadOnly] public Camera MainCamera { private set; get; }

        #region Loading & Transitions
        [Header("Loading & Transitions"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField, ReadOnly] private Coroutine loadRoutine;

        [SerializeField] private Image transitionImage;

        [SerializeField] private TransitionSO transition;

        [SerializeField] private MenuIdEvent OnSetMenuVisibility;

       

        #endregion

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

            transitionImage.enabled = false;
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
            if (level == 0)
            {
                OnSetMenuVisibility.Raise(this, MenuId.StartMenu);
            }
            else
            {
                OnSetMenuVisibility.Raise(this, MenuId.None);                
            }           
            StartCoroutine(StartTransitionRoutine());
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
        private void OnApplicationQuit()
        {
            StopRumble();
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
	public class TransitionManager : MonoBehaviour
	{
        public static TransitionManager Instance { get; private set; }

        [SerializeField] MenuIdEvent menuIdEvent;

        #region Loading & Transitions 
        [Header("LOADING & TRANSITIONS"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] Image transitionImage;

        Coroutine trasitionRoutine;

        [SerializeField] LevelTransition[] levelTransitions;

        [field: SerializeField, ReadOnly] public bool IsTransitioning { get; private set; }
        #endregion

        private void Awake()
        {
            transitionImage.enabled = false;
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void OnLevelWasLoaded(int level)
        {
            if (trasitionRoutine != null)
            {
                trasitionRoutine = null;
                trasitionRoutine = StartCoroutine(TransitionInRoutine(level));
            }
        }

        private void OnValidate()
        {
            for(int i = 0; i < levelTransitions.Length; i++)
            {
                levelTransitions[i].Name = $"Level {i}";
            }
        }

        public void LoadScene(int targetSceneIndex)
        {
            if (trasitionRoutine != null) return;

            trasitionRoutine = StartCoroutine(TransitionOutRoutine(targetSceneIndex));
        }

        public void LoadScreen(EMenuId targetScreen, LevelTransition transition)
        {
            if (trasitionRoutine != null) return;

            trasitionRoutine = StartCoroutine(ScreenTransitionRoutine(targetScreen, transition));
        }

        private IEnumerator TransitionOutRoutine(int targetSceneIndex)
        {
            IsTransitioning = true;

            EventSystem currentEventSystem = EventSystem.current;
            currentEventSystem.sendNavigationEvents = false;

            float frameTime;
            TransitionSO transition;

            transition = levelTransitions[targetSceneIndex].TransitionIn;

            frameTime = transition.Duration / transition.Sprites.Length;

            transitionImage.enabled = true;
            for (int i = 0; i < transition.Sprites.Length; i++)
            {
                transitionImage.sprite = transition.Sprites[i];
                yield return new WaitForSecondsRealtime(frameTime);
            }
            SceneManager.LoadScene(targetSceneIndex);
        }

        private IEnumerator TransitionInRoutine(int targetSceneIndex)
        {
            IsTransitioning = true;

            EventSystem currentEventSystem = EventSystem.current;
            currentEventSystem.sendNavigationEvents = false;

            float frameTime;
            TransitionSO transition;

            transition = levelTransitions[targetSceneIndex].TransitionOut;

            frameTime = transition.Duration / transition.Sprites.Length;

            transitionImage.enabled = true;
            for (int i = transition.Sprites.Length - 1; i > 0; i--)
            {
                transitionImage.sprite = transition.Sprites[i];
                yield return new WaitForSecondsRealtime(frameTime);
            }
            transitionImage.enabled = false;
            currentEventSystem.sendNavigationEvents = true;
            IsTransitioning = false;
            trasitionRoutine = null;
        }

        IEnumerator ScreenTransitionRoutine(EMenuId targetScreen, LevelTransition transition)
        {
            //Out
            IsTransitioning = true;

            EventSystem currentEventSystem = EventSystem.current;
            currentEventSystem.sendNavigationEvents = false;

            float frameTime;

            frameTime = transition.TransitionOut.Duration / transition.TransitionOut.Sprites.Length;

            transitionImage.enabled = true;
            for (int i = 0; i < transition.TransitionOut.Sprites.Length; i++)
            {
                transitionImage.sprite = transition.TransitionOut.Sprites[i];
                yield return new WaitForSecondsRealtime(frameTime);
            }

            menuIdEvent.Raise(this, targetScreen);

            //In
            frameTime = transition.TransitionIn.Duration / transition.TransitionIn.Sprites.Length;

            for (int i = transition.TransitionIn.Sprites.Length - 1; i > 0; i--)
            {
                transitionImage.sprite = transition.TransitionIn.Sprites[i];
                yield return new WaitForSecondsRealtime(frameTime);
            }

            transitionImage.enabled = false;
            currentEventSystem.sendNavigationEvents = true;
            IsTransitioning = false;
            trasitionRoutine = null;
        }

        /*
        #region Testing Methods
        [Button("Test start transition", EButtonEnableMode.Playmode)]
        public void TestRegularTransition()
        {

        }

        [Button("Test load transition", EButtonEnableMode.Playmode)]
        public void TestReverseTransition()
        {

        }
        #endregion
        */
    }

    [Serializable]
    public class LevelTransition
    {
        [HideInInspector] public string Name;
        [field: SerializeField, Expandable] public TransitionSO TransitionIn;
        [field: SerializeField, Expandable] public TransitionSO TransitionOut;
    }
}
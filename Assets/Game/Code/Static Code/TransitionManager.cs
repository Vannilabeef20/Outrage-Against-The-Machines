using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
	public class TransitionManager : MonoBehaviour
	{
        public static TransitionManager Instance { get; private set; }

        #region Loading & Transitions 
        [Header("LOADING & TRANSITIONS"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] Image transitionImage;

        Coroutine loadRoutine;

        [SerializeField] LevelTransition[] levelTransitions;
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
            if (loadRoutine != null)
            {
                loadRoutine = null;
                loadRoutine = StartCoroutine(TransitionInRoutine(level));
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
            if (loadRoutine != null) return;

            loadRoutine = StartCoroutine(TransitionOutRoutine(targetSceneIndex));
        }

        private IEnumerator TransitionOutRoutine(int targetSceneIndex)
        {
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
            loadRoutine = null;
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
        [Serializable]
        class LevelTransition
        {
            [HideInInspector] public string Name;
            [field: SerializeField, Expandable] public TransitionSO TransitionIn;
            [field: SerializeField, Expandable] public TransitionSO TransitionOut;
        }
    }
}
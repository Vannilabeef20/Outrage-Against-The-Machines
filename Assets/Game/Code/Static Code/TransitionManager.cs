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

        private void OnValidate()
        {
            for(int i = 0; i < levelTransitions.Length; i++)
            {
                levelTransitions[i].Name = $"Level {i}";
            }
        }

        public void LoadScene(int targetSceneIndex, TransitionType transitionType)
        {
            if (loadRoutine != null) return;

            switch (transitionType)
            {
                case TransitionType.In :
                    loadRoutine = StartCoroutine(TransitionInRoutine(targetSceneIndex));
                    break;
                case TransitionType.Out:
                    loadRoutine = StartCoroutine(TransitionOutRoutine(targetSceneIndex));
                    break;
            }
        }

        private IEnumerator TransitionInRoutine(int targetSceneIndex)
        {
            int currentLevelIndex;
            float frameTime;
            TransitionSO transition;

            currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

            transition = levelTransitions[currentLevelIndex].TransitionOut;

            frameTime = transition.Duration / transition.Sprites.Length;
            for (int i = transition.Sprites.Length - 1; i > 0; i--)
            {
                transitionImage.sprite = transition.Sprites[i];
                yield return new WaitForSecondsRealtime(frameTime);
            }
            SceneManager.LoadScene(targetSceneIndex);
        }

        private IEnumerator TransitionOutRoutine(int targetSceneIndex)
        {
            int currentLevelIndex;
            float frameTime;
            TransitionSO transition;

            currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

            transition = levelTransitions[currentLevelIndex].TransitionIn;

            frameTime = transition.Duration / transition.Sprites.Length;
            for (int i = 0; i < transition.Sprites.Length; i++)
            {
                transitionImage.sprite = transition.Sprites[i];
                yield return new WaitForSecondsRealtime(frameTime);
            }
            SceneManager.LoadScene(targetSceneIndex);
        }

        [Serializable]
        class LevelTransition
        {
            [HideInInspector] public string Name;
            [field: SerializeField, Expandable] public TransitionSO TransitionIn;
            [field: SerializeField, Expandable] public TransitionSO TransitionOut;
        }
    }
    public enum TransitionType
    {
        In,
        Out,
        None
    }
}
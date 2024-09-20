using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using NaughtyAttributes;

namespace Game
{
    public class CharacterSelectionSwap : MonoBehaviour
    {
        [Header("REFERENCES"), HorizontalLine(2F, EColor.Red)]
        [SerializeField] GameObject[] selectionImageObjects;
        [Space]
        [SerializeField] GameObject joinedUI;
        [SerializeField] GameObject joinUI;
        [SerializeField] GameObject readyObject;
        [SerializeField] GameObject[] speedPoints;
        [SerializeField] GameObject[] defensePoints;

        [field: Header("PARAMETERS"), HorizontalLine(2F, EColor.Orange)]

        [field: SerializeField] public bool IsReady { get; private set; }
        [field: SerializeField] public int PlayerIndex { get; private set; }

        [SerializeField] EPlayerInput leftInput;
        [SerializeField] EPlayerInput rightInput;

        public CharacterOption[] characterOptions;

        [field: SerializeField, ReadOnly] public int SelectionIndex { get; private set; }


        public CharacterOption SelectedCharacter => characterOptions[SelectionIndex];

        private void Start()
        {
            if(GameManager.Instance == null) 
            this.LogError($"Character selection cant be active at the start.", EDebugSubjectFlags.Debug);
        }
        private void OnEnable()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshJoin);

            SelectionIndex = -1;
            RefreshSelection();
        }
        private void OnDisable()
        {
            SelectionIndex = -1;
            GameManager.Instance.UnityInputManager.playerJoinedEvent.RemoveListener(RefreshJoin);
        }
        public void Swap(PlayerGameInput playerGameInput)
        {
            //Checks
            if (IsReady) return;

            if (playerGameInput.Index != PlayerIndex) return;

            if (playerGameInput.Input != leftInput && playerGameInput.Input != rightInput) return;

            if (TransitionManager.Instance.IsTransitioning) return;

            //Left or right
            if (playerGameInput.Input == leftInput)
                SelectionIndex--;
            else if (playerGameInput.Input == rightInput)
                SelectionIndex++;

            //Loop inside range
            if (SelectionIndex < 0) SelectionIndex = characterOptions.Length - 1;

            if (SelectionIndex > characterOptions.Length - 1) SelectionIndex = 0;

            //Update selection visuals
            RefreshSelection();
        }

        public void SwapRight()
        {
            //Checks
            if (IsReady) return;

            if (TransitionManager.Instance.IsTransitioning) return;

            //Right
            SelectionIndex++;

            //Loop inside range
            if (SelectionIndex > characterOptions.Length - 1) SelectionIndex = 0;

            RefreshSelection();
        }

        public void SwapLeft()
        {
            //Checks
            if (IsReady) return;

            if (TransitionManager.Instance.IsTransitioning) return;

            //Left
            SelectionIndex--;

            //Loop inside range
            if (SelectionIndex < 0) SelectionIndex = characterOptions.Length - 1;

            RefreshSelection();
        }

        public void SetSelection(int selectIndex)
        {
            IsReady = false;
            SelectionIndex = selectIndex;

            RefreshSelection();
        }

        public void RefreshJoin(PlayerInput playerInput) //Called on join
        {
            if (playerInput.user.index != PlayerIndex) return;

            IsReady = false;
            SelectionIndex = 0;
            RefreshSelection();
        }

        public void ToggleReady()
        {
            IsReady = !IsReady;
            readyObject.SetActive(IsReady);
        }


        public void RefreshSelection()
        {
            readyObject.SetActive(IsReady);

            //Respective player not joined
            if (SelectionIndex == -1) 
            {
                joinUI.SetActive(true);
                joinedUI.SetActive(false);
                return;
            }

            //Respective player joined

            joinUI.SetActive(false);
            joinedUI.SetActive(true);
            foreach (var imageObject in selectionImageObjects)
            {
                
            }

            for(int i = 0; i < defensePoints.Length; i++)
            {
                if (i < characterOptions[SelectionIndex].defenseValue)
                    defensePoints[i].SetActive(true);
                else 
                    defensePoints[i].SetActive(false);                
            }

            for (int i = 0; i < speedPoints.Length; i++)
            {
                if (i < characterOptions[SelectionIndex].speedValue)
                    speedPoints[i].SetActive(true);
                else
                    speedPoints[i].SetActive(false);
            }

            for(int i = 0; i < selectionImageObjects.Length; i++)
            {
                if(i == SelectionIndex) selectionImageObjects[i].SetActive(true);
                else selectionImageObjects[i].SetActive(false);
            }
        }

        private void OnValidate()
        {
            foreach(var option in characterOptions)
            {
                if (option.prefab == null) option.Name = "Null";

                else option.Name = option.prefab.name;
            }
        }
    }

    [System.Serializable]
    public class CharacterOption
    {
        [HideInInspector] public string Name;

        [Range(0, 3)] public int defenseValue;

        [Range(0, 3)] public int speedValue;

        [ShowAssetPreview] public GameObject prefab;

        [ShowAssetPreview] public Sprite characterIcon;
    }
}

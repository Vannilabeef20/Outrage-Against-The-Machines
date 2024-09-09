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
        [SerializeField] Button swapButton;
        [Space]
        [SerializeField] Image selectionImage;
        [SerializeField] Image defenseArrowImage;
        [SerializeField] Image speedArrowImage;
        [Space]
        [SerializeField] GameObject joinTextObject;
        [SerializeField] GameObject readyObject;

        [field: Header("PARAMETERS"), HorizontalLine(2F, EColor.Orange)]
        [field: SerializeField] public bool IsReady { get; private set; }
        [field: SerializeField] public int PlayerIndex { get; private set; }

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
        public void Swap()
        {
            if (IsReady) return;

            if (TransitionManager.Instance.IsTransitioning) return;

            SelectionIndex++;
            if (SelectionIndex >= characterOptions.Length) 
            {
                SelectionIndex = 0;
            }
            if (SelectionIndex < 0)
            {
                SelectionIndex = characterOptions.Length;
            }
            selectionImage.sprite = characterOptions[SelectionIndex].characterSprite;
            defenseArrowImage.sprite = characterOptions[SelectionIndex].defenseSprite;
            speedArrowImage.sprite = characterOptions[SelectionIndex].speedSprite;
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

            if (SelectionIndex == -1) //Respective player not joined
            {
                joinTextObject.SetActive(true);

                selectionImage.enabled = false;
                defenseArrowImage.enabled = false;
                speedArrowImage.enabled = false;
                swapButton.image.enabled = false;
                swapButton.enabled = false;

                return;
            }

            //Respective player joined

            joinTextObject.SetActive(false);

            swapButton.enabled = true;
            swapButton.image.enabled = true;
            selectionImage.enabled = true;
            defenseArrowImage.enabled = true;
            speedArrowImage.enabled = true;

            selectionImage.sprite = characterOptions[SelectionIndex].characterSprite;
            defenseArrowImage.sprite = characterOptions[SelectionIndex].defenseSprite;
            speedArrowImage.sprite = characterOptions[SelectionIndex].speedSprite;
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

        [ShowAssetPreview] public GameObject prefab;

        [ShowAssetPreview] public Sprite characterSprite;

        [ShowAssetPreview] public Sprite characterIcon;

        [ShowAssetPreview] public Sprite defenseSprite;

        [ShowAssetPreview] public Sprite speedSprite;
    }
}

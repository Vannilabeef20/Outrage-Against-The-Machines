using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using NaughtyAttributes;

namespace Game
{
    public class CharacterSelectionSwap : MonoBehaviour
    {
        [field: SerializeField] public int PlayerIndex { get; private set; }
        [SerializeField] Image selectionImage;
        [SerializeField] Image defenseArrowImage;
        [SerializeField] Image speedArrowImage;
        [SerializeField] Button swapButton;
        [SerializeField] GameObject joinTextObject;

        public CharacterOption[] characterOptions;

        public CharacterOption SelectedCharacter => characterOptions[SelectionIndex];

        [field: SerializeField, ReadOnly] public int SelectionIndex { get; private set; }

        private void Start()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshImage);
            GameManager.Instance.UnityInputManager.playerLeftEvent.AddListener(RefreshImage);
        }
        private void OnEnable()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshImage);
                GameManager.Instance.UnityInputManager.playerLeftEvent.AddListener(RefreshImage);
            }

            if (GameManager.Instance.UnityInputManager.playerCount - 1 < PlayerIndex)
            {
                SelectionIndex = -1;

                joinTextObject.SetActive(true);

                selectionImage.enabled = false;
                defenseArrowImage.enabled = false;
                speedArrowImage.enabled = false;
                swapButton.image.enabled = false;
                swapButton.enabled = false;
            }
            else
            {
                SelectionIndex = 0;

                joinTextObject.SetActive(false);

                swapButton.image.enabled = true;
                swapButton.enabled = true;
                selectionImage.enabled = true;
                defenseArrowImage.enabled = true;
                speedArrowImage.enabled = true;

                selectionImage.sprite = characterOptions[SelectionIndex].characterSprite;
                defenseArrowImage.sprite = characterOptions[SelectionIndex].defenseSprite;
                speedArrowImage.sprite = characterOptions[SelectionIndex].speedSprite;
            }
        }
        private void OnDisable()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.RemoveListener(RefreshImage);
            GameManager.Instance.UnityInputManager.playerLeftEvent.RemoveListener(RefreshImage);
        }
        public void Swap()
        {
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

        public void RefreshImage(PlayerInput playerInput)
        {
            if(playerInput.playerIndex != PlayerIndex)
            {
                return;
            }
            if (GameManager.Instance.UnityInputManager.playerCount - 1 < PlayerIndex)
            {
                //Player not active

                SelectionIndex = -1;

                joinTextObject.SetActive(true);

                selectionImage.enabled = false;
                defenseArrowImage.enabled = false;
                speedArrowImage.enabled = false;
                swapButton.image.enabled = false;
                swapButton.enabled = false;

                return;
            }

            //Player Active

            SelectionIndex = 0;

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

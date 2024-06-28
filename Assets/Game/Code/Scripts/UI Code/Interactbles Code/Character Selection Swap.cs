using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game
{
    public class CharacterSelectionSwap : MonoBehaviour
    {
        [field: SerializeField] public int PlayerIndex { get; private set; }
        [SerializeField] private Image selectionImage;
        [SerializeField] private Image defenseArrowImage;
        [SerializeField] private Image speedArrowImage;
        [SerializeField] private Button swapButton;
        [SerializeField] private GameObject joinTextObject;
        [SerializeField, ShowAssetPreview] private Sprite[] selectionSprites;
        [SerializeField, ShowAssetPreview] private Sprite[] selectionDefenseSprites;
        [SerializeField, ShowAssetPreview] private Sprite[] selectionSpeedSprites;
        [field: SerializeField, ShowAssetPreview] public GameObject[] SelectionPrefabs { get; private set; }
        [field: SerializeField, ReadOnly] public int SelectionIndex { get; private set; }

        private void OnEnable()
        {
            if (GameManager.Instance.UnityInputManager.playerCount - 1 < PlayerIndex)
            {
                SelectionIndex = -1;
                selectionImage.enabled = false;
                defenseArrowImage.enabled = false;
                speedArrowImage.enabled = false;
                joinTextObject.SetActive(true);
                swapButton.image.enabled = false;
                swapButton.enabled = false;
            }
            else
            {
                swapButton.image.enabled = true;
                swapButton.enabled = true;
                SelectionIndex = 0;
                selectionImage.enabled = true;
                defenseArrowImage.enabled = true;
                speedArrowImage.enabled = true;
                joinTextObject.SetActive(false);
                selectionImage.sprite = selectionSprites[SelectionIndex];
                defenseArrowImage.sprite = selectionDefenseSprites[SelectionIndex];
                speedArrowImage.sprite = selectionSpeedSprites[SelectionIndex];
            }
            GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshImage);
        }
        private void OnDisable()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.RemoveListener(RefreshImage);
        }
        public void Swap()
        {
            SelectionIndex++;
            if (SelectionIndex >= 2) //Hard coded
            {
                SelectionIndex = 0;
            }
            if (SelectionIndex < 0)
            {
                SelectionIndex = 2;
            }
            selectionImage.sprite = selectionSprites[SelectionIndex];
            defenseArrowImage.sprite = selectionDefenseSprites[SelectionIndex];
            speedArrowImage.sprite = selectionSpeedSprites[SelectionIndex];
        }

        public void RefreshImage(PlayerInput playerInput)
        {
            if(playerInput.playerIndex != PlayerIndex)
            {
                return;
            }
            if (GameManager.Instance.UnityInputManager.playerCount - 1 < PlayerIndex)
            {
                SelectionIndex = -1;
                selectionImage.enabled = false;
                defenseArrowImage.enabled = false;
                speedArrowImage.enabled = false;
                joinTextObject.SetActive(true);
                swapButton.image.enabled = false;
                swapButton.enabled = false;
            }
            else
            {
                swapButton.enabled = true;
                swapButton.image.enabled = true;
                SelectionIndex = 0;
                selectionImage.enabled = true;
                defenseArrowImage.enabled = true;
                speedArrowImage.enabled = true;
                joinTextObject.SetActive(false);
                selectionImage.sprite = selectionSprites[SelectionIndex];
                defenseArrowImage.sprite = selectionDefenseSprites[SelectionIndex];
                speedArrowImage.sprite = selectionSpeedSprites[SelectionIndex];
            }
        }
    }
}

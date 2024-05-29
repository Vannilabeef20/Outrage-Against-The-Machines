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
        [SerializeField] private Button swapButton;
        [SerializeField] private GameObject joinTextObject;
        [SerializeField, ShowAssetPreview] private Sprite[] selectionSprites;
        [field: SerializeField, ShowAssetPreview] public GameObject[] SelectionPrefabs { get; private set; }
        [field: SerializeField, ReadOnly] public int SelectionIndex { get; private set; }

        private void OnEnable()
        {
            if (GameManager.Instance.UnityInputManager.playerCount - 1 < PlayerIndex)
            {
                SelectionIndex = -1;
                selectionImage.enabled = false;
                joinTextObject.SetActive(true);
                swapButton.interactable = false;
            }
            else
            {
                SelectionIndex = 0;
                selectionImage.enabled = true;
                joinTextObject.SetActive(false);
                swapButton.interactable = true;
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
            if (SelectionIndex >= 3)
            {
                SelectionIndex = 0;
            }
            if (SelectionIndex < 0)
            {
                SelectionIndex = 2;
            }
            selectionImage.sprite = selectionSprites[SelectionIndex];
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
                joinTextObject.SetActive(true);
                swapButton.interactable = false;
            }
            else
            {
                SelectionIndex = 0;
                selectionImage.enabled = true;
                joinTextObject.SetActive(false);
                swapButton.interactable = true;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game
{
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField, ShowAssetPreview] private Sprite[] selectionSprites;
        [SerializeField] private Image[] selectionImages;
        [SerializeField] private GameObject[] joinTextsObjects;
        [SerializeField, ReadOnly] private int[] selectionIndexes = new int[3];
        [SerializeField] private Button confirmButton;

        private void Start()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshImage);
            GameManager.Instance.UnityInputManager.playerLeftEvent.AddListener(RefreshImage);
            RefreshImage();
        }

        private void OnDestroy()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.RemoveListener(RefreshImage);
            GameManager.Instance.UnityInputManager.playerLeftEvent.RemoveListener(RefreshImage);
        }

        public void SwapCharacter(int characterIndex)
        {
            RefreshImage(characterIndex);
            selectionIndexes[characterIndex] ++;
            if (selectionIndexes[characterIndex] >= 3)
            {
                selectionIndexes[characterIndex] = 0;
            }
            if (selectionIndexes[characterIndex] < 0)
            {
                selectionIndexes[characterIndex] = 2;
            }
            selectionImages[characterIndex].sprite = selectionSprites[selectionIndexes[characterIndex]];
        }

        public void RefreshImage(int characterIndex)
        {
            if (GameManager.Instance.UnityInputManager.playerCount != 0)
            {
                confirmButton.interactable = true;
            }
            else
            {
                confirmButton.interactable = false;
            }

            if (GameManager.Instance.UnityInputManager.playerCount - 1 < characterIndex)
            {
                selectionImages[characterIndex].enabled = false;
                joinTextsObjects[characterIndex].SetActive(true);
            }
            else
            {
                selectionImages[characterIndex].enabled = true;
                joinTextsObjects[characterIndex].SetActive(false);
            }
        }
        public void RefreshImage(PlayerInput playerInput)
        {
            if (GameManager.Instance.UnityInputManager.playerCount != 0)
            {
                confirmButton.interactable = true;
            }
            else
            {
                confirmButton.interactable = false;
            }

            for (int i = 0; i < 3; i++)
            {
                if (GameManager.Instance.UnityInputManager.playerCount - 1 < i)
                {
                    selectionImages[i].enabled = false;
                    joinTextsObjects[i].SetActive(true);
                }
                else
                {
                    selectionImages[i].enabled = true;
                    joinTextsObjects[i].SetActive(false);
                }
            }
        }
        public void RefreshImage()
        {
            if (GameManager.Instance.UnityInputManager.playerCount != 0)
            {
                confirmButton.interactable = true;
            }
            else
            {
                confirmButton.interactable = false;
            }

            for (int i = 0; i < 3; i++)
            {
                if (GameManager.Instance.UnityInputManager.playerCount - 1 < i)
                {
                    selectionImages[i].enabled = false;
                    joinTextsObjects[i].SetActive(true);
                }
                else
                {
                    selectionImages[i].enabled = true;
                    joinTextsObjects[i].SetActive(false);
                }
            }
        }

        public void ConfirmSelection()
        {
            GameManager.Instance.PlayerCharacterList.Clear();
            PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
            for (int i = 0; i < playerInputs.Length; i++)
            {
                GameManager.Instance.PlayerCharacterList.Add(new PlayerCharacter(GameManager.Instance.PlayerPrefabs[playerInputs[i].playerIndex],
                    playerInputs[i].playerIndex, playerInputs[playerInputs[i].playerIndex].currentControlScheme, playerInputs[playerInputs[i].playerIndex].devices.ToArray()));
            }
        }
        public void ClearPlayer(MenuId menuId)
        {
            switch(menuId)
            {
                case MenuId.StartMenu:
                    PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
                    foreach (PlayerInput playerInput in playerInputs)
                    {
                        Destroy(playerInput.gameObject);
                    }
                    GameManager.Instance.PlayerCharacterList.Clear();
                    break;
            }
        }
    }
}

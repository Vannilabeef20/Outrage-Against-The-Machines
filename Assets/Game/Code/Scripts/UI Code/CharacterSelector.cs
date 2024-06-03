using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game
{
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button confirmTutorialButton;
        [SerializeField] private CharacterSelectionSwap[] selectionSwaps;

        private void OnEnable()
        {
            confirmButton.interactable = false;
            confirmTutorialButton.interactable = false;
            GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshConfirmButton);
            GameManager.Instance.UnityInputManager.playerLeftEvent.AddListener(RefreshConfirmButton);
        }

        private void OnDisable()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.RemoveListener(RefreshConfirmButton);
            GameManager.Instance.UnityInputManager.playerLeftEvent.RemoveListener(RefreshConfirmButton);
        }

        public void RefreshConfirmButton(PlayerInput playerInput)
        {
            if(GameManager.Instance.UnityInputManager.playerCount < 1)
            {
                confirmButton.interactable = false;
                confirmTutorialButton.interactable = false;
            }
            else
            {
                confirmTutorialButton.interactable = true;
                if(PlayerPrefs.GetInt("IsTutorialCompleted") == 1)
                {
                    confirmButton.interactable = true;
                }
            }
        }

        public void ConfirmSelection()
        {
            GameManager.Instance.PlayerCharacterList.Clear();
            PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
            foreach(var swap in selectionSwaps)
            {
                if(swap.SelectionIndex < 0)
                {
                    continue;
                }
                GameManager.Instance.PlayerCharacterList.Add(new PlayerCharacter(swap.SelectionPrefabs[swap.SelectionIndex], swap.PlayerIndex,
                     playerInputs[swap.PlayerIndex].currentControlScheme, playerInputs[swap.PlayerIndex].devices.ToArray()));
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

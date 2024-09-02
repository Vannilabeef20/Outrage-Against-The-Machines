using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using System.Linq;

namespace Game
{
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField, ReadOnly] private bool wasTutorialCompleted;

        [SerializeField] CharacterSelectionSwap[] selectionSwaps;

        private void Start()
        {
            GameManager.Instance.UnityInputManager?.playerJoinedEvent.AddListener(RefreshConfirmButton);
            GameManager.Instance.UnityInputManager?.playerLeftEvent.AddListener(RefreshConfirmButton);
            if (PlayerPrefs.GetInt("IsTutorialCompleted") == 1)
            {
                wasTutorialCompleted = true;
            }
            else
            {
                wasTutorialCompleted = false;
            }
        }

        private void OnEnable()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshConfirmButton);
                GameManager.Instance.UnityInputManager.playerLeftEvent.AddListener(RefreshConfirmButton);
            }
            if (PlayerPrefs.GetInt("IsTutorialCompleted") == 1)
            {
                wasTutorialCompleted = true;
            }
            else
            {
                wasTutorialCompleted = false;
            }
        }

        private void OnDisable()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.RemoveListener(RefreshConfirmButton);
            GameManager.Instance.UnityInputManager.playerLeftEvent.RemoveListener(RefreshConfirmButton);
        }

        public void RefreshConfirmButton(PlayerInput playerInput)
        {
            /*
            if(GameManager.Instance.UnityInputManager.playerCount < 1)
            {
                confirmButton.interactable = false;
                confirmTutorialButton.interactable = false;
            }
            else
            {
                confirmTutorialButton.interactable = true;
                if(wasTutorialCompleted)
                {
                    confirmButton.interactable = true;
                }
                else
                {
                    tutorialButtonScript.AnimateInteractible(0.1f, 10f, true, false);
                }
            }
            */
        }

        public void ConfirmSelection()
        {
            GameManager.Instance.PlayerCharacterList.Clear();
            PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
            playerInputs = playerInputs.OrderBy(input => input.playerIndex).ToArray();

            foreach(var swap in selectionSwaps)
            {
                if (swap.SelectionIndex < 0)
                {
                    continue;
                }
                PlayerCharacter newPlayer = new PlayerCharacter(
                    swap.SelectedCharacter.prefab,
                    swap.PlayerIndex,
                    swap.SelectedCharacter.characterIcon,
                    playerInputs[swap.PlayerIndex].currentControlScheme,
                    playerInputs[swap.PlayerIndex].devices.ToArray());

                GameManager.Instance.PlayerCharacterList.Add(newPlayer);
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

        [Button("RESET TUTORIAL PREFS")]
        public void ResetTutorialPrefs()
        {
            PlayerPrefs.SetInt("IsTutorialCompleted", 0);
        }
    }
}

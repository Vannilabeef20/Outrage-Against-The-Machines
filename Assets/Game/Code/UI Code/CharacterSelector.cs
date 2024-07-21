using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using System.Linq;

namespace Game
{
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField, ReadOnly] private bool wasTutorialCompleted;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button confirmTutorialButton;
        [SerializeField] private LoadSceneButton tutorialButtonScript;
        [SerializeField] private CharacterSelectionSwap[] selectionSwaps;

        private void OnEnable()
        {
            confirmButton.interactable = false;
            confirmTutorialButton.interactable = false;
            GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshConfirmButton);
            GameManager.Instance.UnityInputManager.playerLeftEvent.AddListener(RefreshConfirmButton);
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
        }

        public void ConfirmSelection()
        {
            GameManager.Instance.PlayerCharacterList.Clear();
            PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
            playerInputs = playerInputs.OrderBy(input => input.playerIndex).ToArray();
            for(int i = 0; i < selectionSwaps.Length; i++)
            {
                if (selectionSwaps[i].SelectionIndex < 0)
                {
                    continue;
                }
                GameManager.Instance.PlayerCharacterList.Add(new PlayerCharacter(selectionSwaps[i].SelectionPrefabs[selectionSwaps[i].SelectionIndex],
                    selectionSwaps[i].PlayerIndex, playerInputs[selectionSwaps[i].PlayerIndex].currentControlScheme,
                    playerInputs[selectionSwaps[i].PlayerIndex].devices.ToArray()));
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

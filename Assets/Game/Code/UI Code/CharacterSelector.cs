using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] EPlayerInput readyInput;
        [SerializeField] EPlayerInput cancelInput;
        [SerializeField, Scene] int targetScene;

        [SerializeField] CharacterSelectionSwap[] selectionSwaps;


        public void ToggleReady(PlayerGameInput playerGameInput)
        {
            if (readyInput != playerGameInput.Input) return;
            if (playerGameInput.Index > selectionSwaps.Length) return;
            if (playerGameInput.Index < 0) return;
            if (TransitionManager.Instance.IsTransitioning) return;

            selectionSwaps[playerGameInput.Index].ToggleReady();

            int readyCount = 0;
            foreach (var selection in selectionSwaps)
            {
                if(selection.SelectionIndex < 0 || selection.IsReady) readyCount++;
            }
            if (readyCount == selectionSwaps.Length) ConfirmSelection();
        }

        public void ConfirmSelection()
        {
            GameManager.Instance.PlayerCharacterList.Clear();
            PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
            playerInputs = playerInputs.OrderBy(input => input.user.index).ToArray();

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

            TransitionManager.Instance.LoadScene(targetScene);
        }
        public void ClearAllPlayers(MenuId menuId)
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

        public void ClearPlayer(PlayerGameInput playerGameInput)
        {
            if (playerGameInput.Input != cancelInput) return;


            if (GameManager.Instance.UnityInputManager.playerCount >= playerGameInput.Index + 1)
            {
                foreach (var swap in selectionSwaps)
                {

                    if (swap.PlayerIndex < playerGameInput.Index) continue;                  

                    if(swap.PlayerIndex + 1 == selectionSwaps.Length)
                    {
                        swap.SetSelection(-1);
                        continue;
                    }
                    swap.SetSelection(selectionSwaps[swap.PlayerIndex + 1].SelectionIndex);
                }
            }

            PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
            playerInputs = playerInputs.OrderBy(input => input.user.index).ToArray();
            Debug.Log(playerInputs[playerGameInput.Index].gameObject.name);
            Destroy(playerInputs[playerGameInput.Index].gameObject);
            if (GameManager.Instance.PlayerCharacterList.Count >= playerGameInput.Index + 1)
                GameManager.Instance.PlayerCharacterList.RemoveAt(playerGameInput.Index);

        }

        [Button("RESET TUTORIAL PREFS")]
        public void ResetTutorialPrefs()
        {
            PlayerPrefs.SetInt("IsTutorialCompleted", 0);
        }
    }
}

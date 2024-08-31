using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using UnityEngine.InputSystem.UI;

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
        [SerializeField, ShowAssetPreview] Sprite[] selectionSprites;
        [SerializeField, ShowAssetPreview] Sprite[] selectionDefenseSprites;
        [SerializeField, ShowAssetPreview] Sprite[] selectionSpeedSprites;
        [field: SerializeField, ShowAssetPreview] public GameObject[] SelectionPrefabs { get; private set; }
        [field: SerializeField, ReadOnly] public int SelectionIndex { get; private set; }

        private void OnEnable()
        {
            GameManager.Instance.UnityInputManager.playerJoinedEvent.AddListener(RefreshImage);
            GameManager.Instance.UnityInputManager.playerLeftEvent.AddListener(RefreshImage);

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

                selectionImage.sprite = selectionSprites[SelectionIndex];
                defenseArrowImage.sprite = selectionDefenseSprites[SelectionIndex];
                speedArrowImage.sprite = selectionSpeedSprites[SelectionIndex];
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
            if (SelectionIndex >= 2) 
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
                //SinglePlayer

                SelectionIndex = -1;

                joinTextObject.SetActive(true);

                selectionImage.enabled = false;
                defenseArrowImage.enabled = false;
                speedArrowImage.enabled = false;
                swapButton.image.enabled = false;
                swapButton.enabled = false;

                if (playerInput.playerIndex != 0) return;

                GameManager.Instance.singlePlayerEventSystem.SetSelectedGameObject(swapButton.gameObject);

                return;
            }

            //Multiplayer

            MultiplayerEventSystem eventSystem;
            switch(playerInput.playerIndex)
            {
                case 0:
                    eventSystem = GameManager.Instance.player1EventSystem;
                    break;
                case 1:
                    eventSystem = GameManager.Instance.player2EventSystem;
                    break;
                case 2:
                    eventSystem = GameManager.Instance.player3EventSystem;
                    break;
                default:
                    eventSystem = GameManager.Instance.player1EventSystem;
                    break;
            }

            eventSystem.SetSelectedGameObject(swapButton.gameObject);
            eventSystem.playerRoot = selectionImage.gameObject;

            SelectionIndex = 0;

            joinTextObject.SetActive(false);

            swapButton.enabled = true;
            swapButton.image.enabled = true;
            selectionImage.enabled = true;
            defenseArrowImage.enabled = true;
            speedArrowImage.enabled = true;

            selectionImage.sprite = selectionSprites[SelectionIndex];
            defenseArrowImage.sprite = selectionDefenseSprites[SelectionIndex];
            speedArrowImage.sprite = selectionSpeedSprites[SelectionIndex];
            
        }
    }
}

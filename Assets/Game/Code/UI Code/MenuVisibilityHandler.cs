using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using UnityEngine.InputSystem.UI;

namespace Game
{
    public class MenuVisibilityHandler : MonoBehaviour
    {


        [SerializeField] MenuId menuId;
        [Space]
        [SerializeField] bool selection = true;
        [Space]
        [SerializeField, ShowIf("selection")] bool allowMultipleSelection;
        [Space]
        [SerializeField, ShowIf("selection")] GameObject singlePlayerSelectableObject;
        [SerializeField, ShowIf("allowMultipleSelection")] GameObject P1_SelectableObject;
        [SerializeField, ShowIf("allowMultipleSelection")] GameObject P2_SelectableObject;
        [SerializeField, ShowIf("allowMultipleSelection")] GameObject P3_SelectableObject;
        [Space]
        [SerializeField, ShowIf("selection")] GameObject singlePlayerRoot;
        [SerializeField, ShowIf("allowMultipleSelection")] GameObject P1_root;
        [SerializeField, ShowIf("allowMultipleSelection")] GameObject P2_root;
        [SerializeField, ShowIf("allowMultipleSelection")] GameObject P3_root;

        EventSystem SinglePlayerEventSystem => GameManager.Instance.singlePlayerEventSystem;
        MultiplayerEventSystem P1eventSystem => GameManager.Instance.player1EventSystem;
        MultiplayerEventSystem P2eventSystem => GameManager.Instance.player2EventSystem;
        MultiplayerEventSystem P3eventSystem => GameManager.Instance.player3EventSystem;

        public void SetVisibility(MenuId eventMenuIds)
        {
            if (!eventMenuIds.HasAnyFlag(menuId))
            {
                gameObject.SetActive(false);
                return;
            }
         
            gameObject.SetActive(true);

            if (!selection) return;

            if(GameManager.Instance.PlayerCharacterList.Count <= 1)
            {
                P1eventSystem.enabled = false;
                P2eventSystem.enabled = false;
                P3eventSystem.enabled = false;
                SinglePlayerEventSystem.SetSelectedGameObject(singlePlayerSelectableObject);
                return;
            }

            SinglePlayerEventSystem.enabled = false;
            P1eventSystem.enabled = true;
            P2eventSystem.enabled = true;
            P3eventSystem.enabled = true;

            if (allowMultipleSelection)
            {
                P1eventSystem.SetSelectedGameObject(P1_SelectableObject);
                P2eventSystem.SetSelectedGameObject(P2_SelectableObject);
                P3eventSystem.SetSelectedGameObject(P3_SelectableObject);
            
                P1eventSystem.playerRoot = P1_root;
                P2eventSystem.playerRoot = P2_root;
                P3eventSystem.playerRoot = P3_root;
            }
            else
            {
                P1eventSystem.SetSelectedGameObject(P1_SelectableObject);
                P2eventSystem.SetSelectedGameObject(null);
                P3eventSystem.SetSelectedGameObject(null);
            
                P1eventSystem.playerRoot = P1_root;
                P2eventSystem.playerRoot = null;
                P3eventSystem.playerRoot = null;
            }           
        }

        private void OnValidate()
        {
            if (selection == false) allowMultipleSelection = false;
        }
    }
}

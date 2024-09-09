using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;

namespace Game
{
    public class MenuVisibilityHandler : MonoBehaviour
    {
        [SerializeField] MenuId menuId;
        [Space]
        [SerializeField] bool selection = true;

        [SerializeField, ShowIf("selection")] GameObject selectableObject;

        public void SetVisibility(MenuId eventMenuIds)
        {
            if (!eventMenuIds.HasAnyFlag(menuId))
            {
                gameObject.SetActive(false);
                return;
            }
         
            gameObject.SetActive(true);

            if (!selection) return;

            EventSystem.current.SetSelectedGameObject(selectableObject);
        }
    }
}

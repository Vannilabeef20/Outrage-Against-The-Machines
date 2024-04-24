using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class MenuVisibilityHandler : MonoBehaviour
    {
        private EventSystem eventSystem;
        [SerializeField] private MenuId menuId;
        [SerializeField] private GameObject defaultSelectableObject;

        private void Awake()
        {
            eventSystem = EventSystem.current;
        }

        public void SetVisibility(MenuId eventMenuIds)
        {
            if (eventMenuIds.HasFlag(menuId))
            {
                gameObject.SetActive(true);
                eventSystem.SetSelectedGameObject(defaultSelectableObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}

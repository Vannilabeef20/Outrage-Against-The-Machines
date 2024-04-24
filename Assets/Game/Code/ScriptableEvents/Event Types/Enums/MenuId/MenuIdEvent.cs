using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New MenuId Event", menuName = "Game Events/MenuId Event")]
    public class MenuIdEvent : BaseGameEvent<MenuId> 
    {
        public override void Raise(object sender, MenuId item)
        {
            base.Raise(sender, item);
            if(item == MenuId.PauseMenu || item == MenuId.PauseOptionsMenu)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            if(item == MenuId.CharacterSelectionMenu)
            {
                GameManager.Instance.UnityInputManager.EnableJoining();
            }
            else
            {
                GameManager.Instance.UnityInputManager.DisableJoining();
            }
        }
    }
}

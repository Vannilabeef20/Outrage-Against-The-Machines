using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Scriptable Object Event of Type "MenuId".<br/>
    /// Relays the received "MenuId" value to all "MenuIdListener" scripts with this reference.<br/>
    /// Its implementation on the listener relays MenuId info to all "MenuVisibilityHandler" Scripts.
    /// <para></para>
    /// Some notable corners being cut:<br/>
    /// If "MenuId" sent is part of a pause menu TimeScale will be set to 0.<br/>
    /// If "MenuId" sent is part of a character selection menu joining will be enabled, else disabled.
    /// </summary>
    [CreateAssetMenu(fileName = "New MenuId Event", menuName = "Game Events/MenuId Event")]
    public class MenuIdEvent : BaseGameEvent<MenuId>
    {
        /// <summary>
        /// Sends "sender" and "menuId" to all listeners with this event's reference.
        /// <para></para>
        /// Some notable corners being cut here ;P.
        /// </summary>
        /// <param name="sender">The object which sent the Raise request.</param>
        /// <param name="menuId">The "MenuId" flags to be sent to all listeners on Raise.</param>
        public override void Raise(object sender, MenuId menuId)
        {
            base.Raise(sender, menuId);
            if (menuId == MenuId.PauseMenu || menuId == MenuId.PauseOptionsMenu)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            if (menuId == MenuId.CharacterSelectionMenu)
            {
                GameManager.Instance.UnityInputManager.EnableJoining();
            }
            else
            {
                GameManager.Instance.UnityInputManager.DisableJoining();
            }
        }
    }
    /// <summary>
    /// Enum flags for menu logical groups.
    /// </summary>
    [System.Flags]
    public enum MenuId
    {
        None = 0,
        StartMenu = 2,
        StartMenuOptions = 4,
        PauseMenu = 8,
        PauseOptionsMenu = 16,
        CharacterSelectionMenu = 32
    }
}

using UnityEngine;

namespace Game
{
    /// <summary>
    /// Listens for Event "MenuIdEvent" of Type "MenuId" and invokes Unity Event Response "UnityMenuIdEvent".
    /// </summary>
    public class MenuIdListener : BaseGameEventListener<EMenuId, MenuIdEvent, UnityMenuIdEvent> { }
}

using UnityEngine.Events;

namespace Game
{
    /// <summary>
    /// Unity event of Type "MenuId".
    /// <para>Will be invoked by a "MenuIdListener" as a response once a "MenuIdEvent" is Raised.</para>
    /// </summary>
    [System.Serializable]
    public class UnityMenuIdEvent : UnityEvent<EMenuId> { }
}
  

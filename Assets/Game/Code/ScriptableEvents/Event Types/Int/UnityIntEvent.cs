using UnityEngine.Events;

namespace Game
{
    /// <summary>
    /// Unity event of Type "int".
    /// <para>Will be invoked by a "IntListener" as a response once a "IntEvent" is Raised.</para>
    /// </summary>
    [System.Serializable]
    public class UnityIntEvent : UnityEvent<int> { }
}

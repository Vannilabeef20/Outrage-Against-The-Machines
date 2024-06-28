using UnityEngine;

namespace Game
{
    /// <summary>
    /// Listens for Event "IntEvent" of Type "int" and invokes Unity Event Response "UnityIntEvent".
    /// </summary>
    public class IntListener : BaseGameEventListener<int, IntEvent, UnityIntEvent> { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Scriptable Object Event of Type "MenuId".<br/>
    /// Relays the received "int" value to all "IntListener" scripts with this reference.
    /// </summary>
    [CreateAssetMenu(fileName = "New Int Event", menuName = "Game Events/Int Event")]
    public class IntEvent : BaseGameEvent<int> {}
}

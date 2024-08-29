using System;
using UnityEngine.Events;

namespace Game
{
    /// <summary>
    /// Unity event of Type "PlayerDeathParams."
    /// <para>Will be invoked by a "PlayerDeathParamsListener" as a response once a "PlayerDeathParamsEvent" is Raised.</para>
    /// </summary>
    [Serializable]
    public class PlayerDeathParamsUnityEvent : UnityEvent<PlayerDeathParams> { }

}
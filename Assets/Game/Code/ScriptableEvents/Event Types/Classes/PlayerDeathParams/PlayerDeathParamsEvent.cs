using UnityEngine;

namespace Game
{
    /// <summary>
    /// Scriptable Object Event of Type "PlayerDeathParams."<br/>
    /// Relays the received "PlayerDeathParams" value to all "PlayerDeathParamsListener" scripts with this reference.<br/>
    /// </summary>
    [CreateAssetMenu(fileName = "New PlayerDeathParams Event", menuName = "SO Events/PlayerDeathParams Event")]
    public class PlayerDeathParamsEvent : BaseGameEvent<PlayerDeathParams> { }

    [System.Serializable]
    /// <summary>
    /// Encapsules death info from player "playerID".
    /// </summary>
    public class PlayerDeathParams
    {
        public int playerIndex;
        public bool isPlayerDead;

        /// <summary>
        /// Builds a new PlayerDeathParams Struct with "_playerID" and "_isPlayerDead."
        /// </summary>
        /// <param name="playerIndex">Target player Inputs index identification.</param>
        /// <param name="_isPlayerDead">Target player death info.</param>
        public PlayerDeathParams(int playerIndex, bool _isPlayerDead)
        {
            this.playerIndex = playerIndex;
            this.isPlayerDead = _isPlayerDead;
        }
    }
}
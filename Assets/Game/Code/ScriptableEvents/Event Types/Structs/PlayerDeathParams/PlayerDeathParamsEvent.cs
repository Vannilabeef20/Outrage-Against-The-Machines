using UnityEngine;

namespace Game
{
    /// <summary>
    /// Scriptable Object Event of Type "PlayerDeathParams."<br/>
    /// Relays the received "PlayerDeathParams" value to all "PlayerDeathParamsListener" scripts with this reference.<br/>
    /// </summary>
    [CreateAssetMenu(fileName = "New PlayerDeathParams Event", menuName = "Game CustomEvents/PlayerDeathParams Event")]
    public class PlayerDeathParamsEvent : BaseGameEvent<PlayerDeathParams> { }

    [System.Serializable]
    /// <summary>
    /// Encapsules death info from player "playerID".
    /// </summary>
    public struct PlayerDeathParams
    {
        [Tooltip("Player Inputs index identification")]
        public int playerID;
        public bool isPlayerDead;

        /// <summary>
        /// Builds a new PlayerDeathParams Struct with "_playerID" and "_isPlayerDead."
        /// </summary>
        /// <param name="_playerID">Target player Inputs index identification.</param>
        /// <param name="_isPlayerDead">Target player death info.</param>
        public PlayerDeathParams(int _playerID, bool _isPlayerDead)
        {
            this.playerID = _playerID;
            this.isPlayerDead = _isPlayerDead;
        }
    }
}
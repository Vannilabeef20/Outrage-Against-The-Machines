using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New PlayerDeathParams Event", menuName = "Game Events/PlayerDeathParams Event")]
    public class PlayerDeathParamsEvent : BaseGameEvent<PlayerDeathParams> { }

    public struct PlayerDeathParams
    {
        public int playerID;
        public bool isPlayerDead;

        public PlayerDeathParams(int _playerID, bool _isPlayerDead)
        {
            this.playerID = _playerID;
            this.isPlayerDead = _isPlayerDead;
        }
    }
}
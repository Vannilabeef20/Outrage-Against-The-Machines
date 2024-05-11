using UnityEngine;
using DG.Tweening;

namespace Game
{
    [CreateAssetMenu(fileName = "New Player Special Event", menuName = "Game Events/Player Special Event")]
    public class PlayerSpecialParamsEvent : BaseGameEvent<PlayerSpecialParams> { }

    public struct PlayerSpecialParams
    {
        public int playerID;
        public float formerSpecialPercent;
        public float newSpecialPercent;
        public PlayerSpecialParams(int PlayerID, float FormerSpecialPercent, float NewSpecialPercent)
        {
            this.playerID = PlayerID;
            this.formerSpecialPercent = FormerSpecialPercent;
            this.newSpecialPercent = NewSpecialPercent;
        }
    }
    
}
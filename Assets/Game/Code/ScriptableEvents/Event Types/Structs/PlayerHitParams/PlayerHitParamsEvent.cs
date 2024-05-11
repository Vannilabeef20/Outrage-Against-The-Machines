using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game
{
    [CreateAssetMenu(fileName = "New PlayerHitParamsEvent", menuName = "Game Events/PlayerHitParams Event")]
    public class PlayerHitParamsEvent : BaseGameEvent<PlayerHitParams> { }

    public struct PlayerHitParams
    {
        public int playerID;
        public float formerHealthPercent;
        public float newHealthPercent;

        public PlayerHitParams (int playerID, float FormerHealthPercent, float NewHealthPercent)
        {
            this.playerID = playerID;
            this.formerHealthPercent = FormerHealthPercent;
            this.newHealthPercent = NewHealthPercent;
        }

    }
}

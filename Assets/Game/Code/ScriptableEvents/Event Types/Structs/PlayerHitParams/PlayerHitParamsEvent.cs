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
        public float healthLerpDelay;
        public float healthLerpDuration;
        public Ease healthLerpEasing;


        public PlayerHitParams (int playerID, float FormerHealthPercent, float NewHealthPercent, float HealthLerpDelay, float HealthLerpDuration, Ease HealthLerpEasing)
        {
            this.playerID = playerID;
            this.formerHealthPercent = FormerHealthPercent;
            this.newHealthPercent = NewHealthPercent;
            this.healthLerpDelay = HealthLerpDelay;
            this.healthLerpDuration = HealthLerpDuration;
            this.healthLerpEasing = HealthLerpEasing;
        }

    }
}

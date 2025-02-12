using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Gets the closest available player.
    /// </summary>
    [System.Serializable]
    public class TargetClosest : BaseTargeting
    {
        public override Transform GetTarget(Vector3 currentPosition)
        {
            List<Transform> playerTransformList = new();
            foreach(var player in GameManager.Instance.PlayerCharacterList)
            {
                if (!player.isDead)
                {
                    playerTransformList.Add(player.Transform);
                }
            }
            if(playerTransformList.Count > 0)
            {
                playerTransformList = playerTransformList.OrderBy(player => Vector3.Distance
                (currentPosition, player.position)).ToList<Transform>();
                return playerTransformList[0];
            }
            else
            {
                return null;
            }          
        }
    }
}

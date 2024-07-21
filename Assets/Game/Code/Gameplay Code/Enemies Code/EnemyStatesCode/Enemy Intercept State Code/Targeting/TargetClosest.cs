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
        public override GameObject GetTarget(Vector3 currentPosition)
        {
            List<GameObject> playerList = new();
            foreach(var player in GameManager.Instance.PlayerCharacterList)
            {
                if (player.isPlayerActive)
                {
                    playerList.Add(player.GameObject);
                }
            }
            if(playerList.Count > 0)
            {
                playerList = playerList.OrderBy(player => Vector3.Distance
                (currentPosition, player.transform.position)).ToList<GameObject>();
                return playerList[0];
            }
            else
            {
                return null;
            }          
        }
    }
}

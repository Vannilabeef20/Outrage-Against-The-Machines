using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class TargetClosest : BaseTargeting
    {
        public override GameObject GetTarget(Vector3 currentPosition)
        {
            List<GameObject> playerList = new();
            for (int i = 0; i < GameManager.Instance.PlayerObjectArray.Length; i++)
            {
                if (GameManager.Instance.isPlayerActiveArray[i])
                {
                    playerList.Add(GameManager.Instance.PlayerObjectArray[i]);
                }
            }
            playerList = playerList.OrderBy(player => Vector3.Distance
            (currentPosition, player.transform.position)).ToList<GameObject>();
            return playerList[0];
        }
    }
}

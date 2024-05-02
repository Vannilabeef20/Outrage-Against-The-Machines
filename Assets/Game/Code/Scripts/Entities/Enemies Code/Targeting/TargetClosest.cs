using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class TargetClosest : BaseTargeting
    {
        [SerializeField, ReadOnly, ShowAssetPreview] private GameObject[] playerGameobjects;
        public override GameObject GetTarget(Vector3 currentPosition, GameObject[] players)
        {
            playerGameobjects = GameplayManager.Instance.PlayerObjectArray;
            playerGameobjects = playerGameobjects.OrderBy(player => Vector3.Distance
            (currentPosition, player.transform.position)).ToArray<GameObject>();
            return playerGameobjects[0];
        }
    }
}

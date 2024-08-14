using System;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [Serializable]
    public class TargetNull : BaseTargeting
    {
        public override GameObject GetTarget(Vector3 currentPosition)
        {
            return null;
        }
    }
}
using System;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [Serializable]
    public class TargetNull : BaseTargeting
    {
        public override Transform GetTarget(Vector3 currentPosition)
        {
            return null;
        }
    }
}
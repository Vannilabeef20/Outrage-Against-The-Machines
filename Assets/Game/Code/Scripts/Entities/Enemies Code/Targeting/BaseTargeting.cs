using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class BaseTargeting : MonoBehaviour
    {
        public abstract GameObject GetTarget(Vector3 currentPosition, GameObject[] players);
    }
}

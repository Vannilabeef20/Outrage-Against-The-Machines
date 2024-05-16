using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    public class MoveDestination : MonoBehaviour
    {
        [SerializeField] private Vector3[] destinationPoints;
        [SerializeField] private float[] speeds;
        [SerializeField] private int currentTargetDestinationIndex;

        void Update()
        {

        }
    }
}

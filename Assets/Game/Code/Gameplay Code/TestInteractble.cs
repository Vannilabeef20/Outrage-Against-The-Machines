using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class TestInteractble : BaseInteractble
    {
        public override void Interact(int playerNumber)
        {
            Debug.Log($"Player {playerNumber + 1} interacted");
        }
    }
}
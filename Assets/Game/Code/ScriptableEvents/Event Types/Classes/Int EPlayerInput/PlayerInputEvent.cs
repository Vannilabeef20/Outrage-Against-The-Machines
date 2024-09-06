using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New PlayerInput Event", menuName = "Player/Input Event")]
    public class PlayerInputEvent : BaseGameEvent<PlayerGameInput> { }

    [Serializable]
    public class PlayerGameInput 
    {
        [field: SerializeField] public int Index { get; private set; }

        [field: SerializeField] public EPlayerInput Input { get; private set; }

        public PlayerGameInput(int playerIndex, EPlayerInput playerInput)
        {
            Index = playerIndex;
            Input = playerInput;
        }

        public override string ToString()
        {
            return $"Index: {Index}, Input {Input}.";
        }
    }
}
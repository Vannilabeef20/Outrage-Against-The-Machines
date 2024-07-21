using System;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [Serializable]
    public class PlayerAttack
    {
        [field: SerializeField] public PlayerAttackSO Parameters { get; private set; }

    }
}
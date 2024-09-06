using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [CreateAssetMenu(fileName = "New IntFloat Event", menuName = "SO Events/IntFloat Event")]
    public class IntFloatEvent : BaseGameEvent<IntFloat> { }

    [Serializable]
    public class IntFloat
    {
        [field: SerializeField, ReadOnly] public int Int { get; private set; }
        [field: SerializeField, ReadOnly] public float Float { get; private set; }

        /// <summary>
        /// Builds a new PlayerDeathParams Struct with "_playerID" and "_isPlayerDead."
        /// </summary>
        /// <param name="newInt">Target player Inputs index identification.</param>
        public IntFloat(int newInt, float newFloat)
        {
            Int = newInt;
            Float = newFloat;
        }

        public override string ToString()
        {
            return $"Int: {Int}, Float: {Float}.";
        }
    }
}
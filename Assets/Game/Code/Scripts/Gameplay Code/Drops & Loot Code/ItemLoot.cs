using System;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [Serializable]
    public class ItemLoot
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, ShowAssetPreview] public GameObject Item { get; private set; }
        [field: SerializeField, Range(0f,100f)] public float ProbabilityWeight { get; private set; }

        [ReadOnly, AllowNesting, Range(0f, 100f)] public float ProbabilityPercent;
        [ReadOnly, MinMaxSlider(0f,100f)] public Vector2 ProbabilityRange;
    }
}

using System;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Defines a possible item loot in a "LootTable"
    /// </summary>
    [Serializable]
    public class ItemLoot
    {
        /// <summary>
        /// The name of the loot item.
        /// </summary>
        [field: SerializeField] public string Name { get; private set; }

        /// <summary>
        /// The prefab of the loot item.
        /// </summary>
        [field: SerializeField, ShowAssetPreview] public GameObject Item { get; private set; }

        /// <summary>
        /// The probability wheight of this item.
        /// <para>The higher the number, the more likely to be drawn.</para>
        /// </summary>
        [field: SerializeField, Range(0f,100f)] public float ProbabilityWeight { get; private set; }

        [Tooltip("The percentual chance for this item to be drawn.")]
        [ReadOnly, AllowNesting, Range(0f, 100f)] public float ProbabilityPercent;

        [Tooltip("The number range that the Random.range has to generate for this item to be drawn.")]
        [ReadOnly, MinMaxSlider(0f,100f)] public Vector2 ProbabilityRange;
    }
}

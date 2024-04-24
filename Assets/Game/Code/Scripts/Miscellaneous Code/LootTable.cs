using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [Serializable]
    public class LootTable 
    {
        [SerializeField] private ItemLoot[] itemDrops;

        [SerializeField ,ReadOnly, AllowNesting] private float totalWeight;

        public void ValidateTable()
        {
            if(itemDrops != null && itemDrops.Length > 0)
            {
                float currentProbabilityWeightMax = 0f;
                foreach (var drop in itemDrops)
                {
                    drop.ProbabilityRange.x = currentProbabilityWeightMax;
                    currentProbabilityWeightMax += drop.ProbabilityWeight;
                    drop.ProbabilityRange.y = currentProbabilityWeightMax;
                }
                totalWeight = currentProbabilityWeightMax;
                foreach(var drop in itemDrops)
                {
                    drop.ProbabilityPercent = (drop.ProbabilityWeight / totalWeight) * 100;
                }
            }
        }

        public GameObject PickRandomDrop()
        {
            GameObject pickedItem = null;
            float pickedNumber = UnityEngine.Random.Range(0, totalWeight);

            foreach(var drop in itemDrops)
            {
                if(pickedNumber > drop.ProbabilityRange.x && pickedNumber < drop.ProbabilityRange.y)
                {
                    pickedItem = drop.Item;
                    break;
                }
            }
            return pickedItem;
        }
    }
}

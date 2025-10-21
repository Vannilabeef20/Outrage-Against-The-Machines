using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Creates a prop that can be broken to gather loot.
    /// </summary>
    [SelectionBase]
    public class BreakableLootProp : MonoBehaviour, IDamageble
    {
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [SerializeField, Required] Animator animator;
        [SerializeField, Required] Transform lootSpawnPoint;

        [Header("PARAMETERS & VARIABLES"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Trigger)] int breakTrigger;

        [SerializeField] LootTable lootTable;
        [SerializeField] ScrapDropper moneyDrop;

        [SerializeField, ReadOnly] bool hasBeenBroken;

        private void Awake()
        {
            lootTable.ValidateTable();
        }

        public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            if (hasBeenBroken) return;

            animator.SetTrigger(breakTrigger);
        }

        public void SpawnLoot()
        {
            Instantiate(lootTable.PickRandomDrop(), lootSpawnPoint.position, Quaternion.identity);
            moneyDrop.SpawnAllScrap();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }     

#if UNITY_EDITOR
        private void OnValidate()
        {
            lootTable.ValidateTable();
        }

#endif
    }
}
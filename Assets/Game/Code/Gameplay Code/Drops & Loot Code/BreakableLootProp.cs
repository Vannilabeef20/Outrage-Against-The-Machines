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
        [SerializeField, Required] GameObject collisionCollider;

        [Tooltip("This BreakableLootProp's spriteRenderer.")]
        [SerializeField] SpriteRenderer spriteRenderer;

        [Header("PARAMETERS & VARIABLES"), HorizontalLine(2f, EColor.Orange)]

        [Tooltip("True if the box has been broken already.")]
        [SerializeField, ReadOnly] bool hasBeenBroken;

        [Tooltip("This BreakableLootProp's break animation sprites.")]
        [SerializeField, ShowAssetPreview] Sprite[] animationSprites;

        [Tooltip("How long will the break animation last for.")]
        [SerializeField] float animationDuration;

        [SerializeField] Color fadeFlickerColor;
        [SerializeField] float fadeFlickerDuration;
        [SerializeField] float fadeFlickerLenght;

        [Tooltip("The animation frame which loot will spawn.")]
        [SerializeField] int FrameToSpawn;

        [Tooltip("Manages the chances for loot drops.")]
        [SerializeField] LootTable lootTable;
        [SerializeField] ScrapDropper moneyDrop;

        private void Awake()
        {
            lootTable.ValidateTable();
        }

        public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            if (hasBeenBroken) return;

            StartCoroutine(BreakRoutine());
        }

        private IEnumerator BreakRoutine()
        {
            hasBeenBroken = true;
            collisionCollider.SetActive(false);
            float frameTime = animationDuration / animationSprites.Length;
            for (int i = 1; i < animationSprites.Length; i++) //Play break animation
            {
                yield return new WaitForSeconds(frameTime);
                spriteRenderer.sprite = animationSprites[i];
                if (i == FrameToSpawn) //Spawn loot at the right frame
                {
                    Instantiate(lootTable.PickRandomDrop(), lootSpawnPoint.position, Quaternion.identity);
                    moneyDrop.SpawnAllScrap();
                }
            }
            float time = Time.time;
            float startTime = Time.time;
            Color originalColor = spriteRenderer.color;
            while (time - startTime < fadeFlickerDuration) //Flicker fade
            {
                yield return new WaitForSeconds(fadeFlickerLenght);
                if (spriteRenderer.color == originalColor)
                {
                    spriteRenderer.color = fadeFlickerColor;
                }
                else
                {
                    spriteRenderer.color = originalColor;
                }
                time = Time.time;
            }
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
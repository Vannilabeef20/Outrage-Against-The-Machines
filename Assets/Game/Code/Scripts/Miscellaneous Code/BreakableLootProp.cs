using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class BreakableLootProp : MonoBehaviour
	{
        [SerializeField] private bool hasBeenBroken;
        [SerializeField] private LootTable lootTable;
        [SerializeField] private LayerMask hostileLayers;
        [SerializeField] private Transform lootSpawnPoint;
        [SerializeField] private BoxCollider collisionCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField, ShowAssetPreview] private Sprite[] animationSprites;
        [SerializeField] private float animationDuration;
        [SerializeField] private Color fadeFlickerColor;
        [SerializeField] private float fadeFlickerDuration;
        [SerializeField] private float fadeFlickerLenght;


        private void Awake()
        {
            lootTable.ValidateTable();
        }
        private void OnTriggerEnter(Collider other)
        {
            if(hasBeenBroken)
            {
                return;
            }
            if(hostileLayers.ContainsLayer(other.gameObject.layer))
            {
                StartCoroutine(BreakRoutine());
            }
        }

        private IEnumerator BreakRoutine()
        {
            hasBeenBroken = true;
            collisionCollider.enabled = false;
            Instantiate(lootTable.PickRandomDrop(), lootSpawnPoint.position, Quaternion.identity);
            float frameTime =  animationDuration/ animationSprites.Length;
            for (int i = 1; i < animationSprites.Length; i++)
            {
                yield return new WaitForSeconds(frameTime);
                spriteRenderer.sprite = animationSprites[i];
            }
            float time = Time.time;
            float startTime = Time.time;
            Color originalColor = spriteRenderer.color;
            while(time - startTime < fadeFlickerDuration)
            {
                yield return new WaitForSeconds(fadeFlickerLenght);
                if(spriteRenderer.color == originalColor)
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
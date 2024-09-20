using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using FMODUnity;

namespace Game
{
    /// <summary>
    /// Defines an item pickup that can be picked up by the player.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    public class ItemDrop : MonoBehaviour
    {
        #region REFERENCES
        [field: Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]
        [field: SerializeField] public GameObject Prefab { get; private set; }

        [Tooltip("The item drop visual transform.\n" +
            "Used for making the item float.")]
        [SerializeField] Transform itemSpriteTransform;

        [SerializeField] StudioEventEmitter pickupEmitter;
        #endregion

        #region PARAMETERS & VARIABLES
        [Header("PARAMETERS & VARIABLES"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField, Tag] string[] playerTags;

        [Tooltip("Which pickup effects will be applied on pickup.")]
        [SerializeReference, SubclassSelector] BaseItemDropEffect[] pickupEffects;

        [Tooltip("How far up and down the itemDrop will float.")]
        [SerializeField] Vector3 floatRange = new Vector3 (0f,0.2f,0f);

        [Tooltip("How long(seconds) does a float cycle takes.")]
        [SerializeField] float floatDuration = 1.5f;

        [Tooltip("The easing used on the float movement.")]
        [SerializeField] Ease floatEase = Ease.InOutSine;
        #endregion

        void Start()
        {
            //Makes the "itemSpriteTransform" float up and down
            itemSpriteTransform.DOMove(floatRange + itemSpriteTransform.position,
                floatDuration * 0.5f).SetEase(floatEase).SetLoops(-1, LoopType.Yoyo);
        }

        void OnTriggerEnter(Collider other)
        {
            for (int i = 0; i < playerTags.Length; i++)
            {
                if (other.gameObject.CompareTag(playerTags[i]))
                {
                    pickupEmitter.Play();
                    foreach(var effect in pickupEffects)
                    {
                        effect.ApplyEffect(other.gameObject);
                    }
                    GameManager.Instance.PlayerCharacterList[i].StoreItem(Prefab);
                    DOTween.Kill(gameObject.transform);
                    Destroy(gameObject);
                    break;
                }
            }        
        }
    }
}

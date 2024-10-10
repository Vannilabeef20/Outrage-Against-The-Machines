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
        [field: SerializeField, ShowAssetPreview()] public Sprite Icon { get; private set; }

        [Tooltip("The item drop visual transform.\n" +
            "Used for making the item float.")]
        [SerializeField] Transform itemSpriteTransform;

        [SerializeField] IntEvent itemEvent;

        [SerializeField] StudioEventEmitter pickEmitter;
        [SerializeField] StudioEventEmitter pickupEmitter;
        #endregion

        #region PARAMETERS & VARIABLES
        [Header("PARAMETERS & VARIABLES"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] bool useImmediatly;

        [Tooltip("Which pickup effects will be applied on pickup.")]
        [SerializeReference, SubclassSelector] BaseItemDropEffect[] pickupEffects;

        [Tooltip("How far up and down the itemDrop will float.")]
        [SerializeField] Vector3 floatRange = new Vector3(0f, 0.2f, 0f);

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
            PickUp(other.gameObject);
        }

        void PickUp(GameObject targetPlayer)
        {
            //Search player per tag
            for (int i = 0; i < GameManager.Instance.PlayerTags.Length; i++)
            {
                if (targetPlayer.CompareTag(GameManager.Instance.PlayerTags[i]))
                {
                    if (useImmediatly)
                    {
                        Use(i);
                        return;
                    }

                    if (GameManager.Instance.PlayerCharacterList[i].HasItemStored) break;

                    GameManager.Instance.PlayerCharacterList[i].StoreItem(gameObject, Icon);
                    pickEmitter.Play();
                    gameObject.SetActive(false);
                    itemEvent.Raise(this, i);
                    return;
                }
            }
        }

        public void Use(int playerIndex)
        {
            pickupEmitter.Play();
            foreach (var effect in pickupEffects)
            {
                effect.ApplyEffect(GameManager.Instance.
                    PlayerCharacterList[playerIndex].GameObject);
            }
            DOTween.Kill(gameObject.transform);
            GameManager.Instance.PlayerCharacterList[playerIndex].RemoveItem();
            itemEvent.Raise(this, playerIndex);
            Destroy(gameObject);
        }
    }
}


using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    /// <summary>
    /// Mimics a box falling behaviour.
    /// </summary>
	public class FallingBox : MonoBehaviour
	{
        #region REFERENCES
        [field: Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] AnimateImage animateImage;
        [SerializeField] SpriteRenderer boxRenderer;
        [SerializeField] SpriteRenderer highlightRenderer;
        [SerializeField] BoxCollider boxCollider;
        [SerializeField] Transform boxTransform;
        [SerializeField] Transform shadowTransform;
        [SerializeField] StudioEventEmitter impactEmitter;
        [SerializeField] StudioEventEmitter fallEmitter;

        #endregion

        #region PARAMETERS
        [field: Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]

        [Tooltip("How fast this box will fall.")]
        [SerializeField] FallingBoxSpeedSO fallingSpeedSO;
        [Tooltip("Sort order for the broken box after impact.")]
        [SerializeField] int postImpactSortOrder = -2;
        [Space]
        [Tooltip("One of the colors the highlight will be set to intermittently.")]
        [SerializeField] Color highlightColor1;
        [Tooltip("One of the colors the highlight will be set to intermittently.")]
        [SerializeField] Color highlightColor2;
        [Tooltip("How long the highlight takes to change from one color to another.")]
        [SerializeField] float highlightLenght;
        [Space]
        [Tooltip("How long the fade effect will last.")]
        [SerializeField] float fadeLenght;
        [Tooltip("How long the fade effect will take to start.")]
        [SerializeField] float fadeDelay;

        #endregion

        #region VARIABLES
        [field: Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]

        [Tooltip("Whether this box has fallen already.")]
        [SerializeField, ReadOnly] bool fell;
        [Tooltip("This box's spawn position.")]
        [SerializeField, ReadOnly] Vector3 startPos;
        [Tooltip("This box's target position.")]
        [SerializeField, ReadOnly] Vector3 finalPos;
        [Tooltip("This box's color.")]
        [SerializeField, ReadOnly] Color defaultBoxColor;
        [Tooltip("How many seconds ago has the highlight color changed.")]
        [SerializeField, ReadOnly] float highlightTimer;
        [Tooltip("How many seconds ago the fade process has started.")]
        [SerializeField, ReadOnly] float fadeTimer;

        #endregion

        #region UNITY METHODS
        private void Awake()
        {
            fadeTimer = - fadeDelay;
            defaultBoxColor = boxRenderer.color;
            highlightRenderer.color = highlightColor1;
            startPos = boxTransform.position;
            finalPos = new Vector3(boxTransform.position.x, boxTransform.position.z, boxTransform.position.z);
            shadowTransform.position = finalPos;
            animateImage.pause = true;
        }
        private void Update()
        {
            ManageShadowHighlight();
            ManageBoxFade();
            ManageShadowSize();
            ManageImpact();
        }
        #endregion

        #region METHODS



        /// <summary>
        /// Switches the "highlightRenderer" color every "highlightLenght" seconds.
        /// </summary>
        private void ManageShadowHighlight()
        {
            highlightTimer += Time.deltaTime;
            if (highlightTimer > highlightLenght)
            {
                if (highlightRenderer.color == highlightColor1)
                {
                    highlightRenderer.color = highlightColor2;
                }
                else
                {
                    highlightRenderer.color = highlightColor1;
                }
                highlightTimer = 0;
            }
        }

        /// <summary>
        /// Fades the "boxRenderer" after "fadeDelay" taking "fadeLenght" seconds to complete.
        /// </summary>
        private void ManageBoxFade()
        {
            if (fell)
            {
                fadeTimer += Time.deltaTime;
                boxRenderer.color = defaultBoxColor.ChangeAlpha(1 - fadeTimer.Map(fadeDelay, fadeDelay + fadeLenght));
            }
            if(fadeTimer > fadeDelay + fadeLenght)
            {
                fallEmitter.EventInstance.release();
                impactEmitter.EventInstance.release();
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Expands the shadow size the closer it gets from the ground.
        /// </summary>
        private void ManageShadowSize()
        {
            if (!fell)
            {
                shadowTransform.localScale = boxTransform.position.magnitude.Map(startPos.magnitude, finalPos.magnitude) * Vector3.one;
                boxTransform.position += fallingSpeedSO.FallingSpeed * Time.deltaTime * Vector3.down;
            }
        }

        /// <summary>
        /// Detects wheter the box has reached ground and does some stuff after.
        /// </summary>
        private void ManageImpact()
        {
            if (boxTransform.position.y < boxTransform.position.z && fell == false)
            {
                boxCollider.enabled = false;
                animateImage.pause = false;
                boxTransform.position = new Vector3(boxTransform.position.x, boxTransform.position.z, boxTransform.position.z);
                fallEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                impactEmitter.Play();
                shadowTransform.localScale = Vector3.zero;
                boxRenderer.sortingOrder = postImpactSortOrder;
                fell = true;
            }
        }

        #endregion
    }
}
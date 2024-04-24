using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class ApplyParallax : MonoBehaviour
    {
        [Tooltip("Sprite renderer that will be used to get the targetSprite and the spriteDimensions.\n" +
            "HAS TO BE FROM THIS GAMEOBJECT!")]
        [SerializeField, ReadOnly] private SpriteRenderer targetSpriteRenderer;
        [Tooltip("Visual preview for the sprite that will be applied parallax.")]
        [SerializeField, ShowAssetPreview(128, 128)] private Sprite targetSprite;
        [Tooltip("When true, the target sprite will be repeated seamlessly infinitly")]
        [SerializeField] private bool RepeatSprite = true;
        [Tooltip("Determines how much faster (factor > 0) or slower (factor < 0)\n" +
            " the target sprite will move in the X axis than the camera movement.")]
        [Range(-1f, 1f), SerializeField] private float parallaxFactorX;
        [Tooltip("Determines how much faster (factor > 0) or slower (factor < 0)\n" +
            " the target sprite will move in the Y axis than the camera movement.")]
        [Range(-1f, 1f), SerializeField] private float parallaxFactorY;
        [Tooltip("Sprites dimensions in the X and Y axis, used to seamlessly repeat the sprites.")]
        [SerializeField, ReadOnly] private Vector2 spriteDimensions;
        [Tooltip("Cached variable, determines how much the sprite will move each frame.")]
        [SerializeField, ReadOnly] private Vector2 parallaxTranslocation;

        private void Awake()
        {
            targetSpriteRenderer = GetComponent<SpriteRenderer>();
            targetSprite = targetSpriteRenderer.sprite;
            targetSpriteRenderer.size = new Vector2(targetSpriteRenderer.size.x * 6, targetSpriteRenderer.size.y);
            spriteDimensions = targetSpriteRenderer.bounds.size;
        }

        public void ApplyParallaxMovement(Vector2 delta, Vector2 cameraCurrentPosition) //Delta, CurrentPosition
        {
            //Moves the sprite
            parallaxTranslocation = new Vector2(delta.x * parallaxFactorX, delta.y * parallaxFactorY);
            transform.position += (Vector3)parallaxTranslocation;
   
            //Checks if the sprite is allowed be moved
            if(!RepeatSprite)
            {
                return;
            }

            //Repeats the sprite
            if ((cameraCurrentPosition.x - transform.position.x) >= spriteDimensions.x / 3)
            {
                transform.position += new Vector3((spriteDimensions.x / 3f), 0f);
            }
            else if ((cameraCurrentPosition.x - transform.position.x) <= -spriteDimensions.x / 3)
            {
                transform.position += new Vector3((-spriteDimensions.x / 3f), 0f);
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Game
{
    public class AnimateImage : MonoBehaviour
    {
        private enum EAnimationTarget
        {
            SpriteRenderer,
            Image
        }

        [SerializeField] private EAnimationTarget targetType;
        [SerializeField, ShowIf("targetType", EAnimationTarget.SpriteRenderer)] private SpriteRenderer spriteRenderer;
        [SerializeField, ShowIf("targetType", EAnimationTarget.Image)] private Image image;
        [SerializeField] private bool loop = true;
        [SerializeField, Min(0f)] private float animationDuration;
        [SerializeField] private bool DestroyOnFinish;
        [SerializeField, ShowAssetPreview] private Sprite[] sprites;
        [SerializeField, ReadOnly] private float timer;
        [SerializeField, ReadOnly] private int currentImageIndex;
        public bool pause;

        private void Update()
        {
            if(pause)
            {
                return;
            }
            timer += Time.deltaTime;
            float frameDuration = animationDuration / sprites.Length;
            if(timer < frameDuration)
            {
                return;
            }
            timer = 0;
            currentImageIndex++;
            if(currentImageIndex >= sprites.Length)
            {
                if(DestroyOnFinish)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    if(loop)
                    {
                        currentImageIndex = 0;
                    }
                    else
                    {
                        currentImageIndex = sprites.Length - 1; 
                        pause = true;
                    }
                }
            }
            if (targetType == EAnimationTarget.Image)
            {
                image.sprite = sprites[currentImageIndex];
            }
            else
            {
                spriteRenderer.sprite = sprites[currentImageIndex];
            }

        }
    }
}

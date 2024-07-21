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

        [SerializeField, ShowIf("targetType", EAnimationTarget.SpriteRenderer)] private SpriteRenderer spriteRenderer;
        [SerializeField, ShowIf("targetType", EAnimationTarget.Image)] private Image image;
        [SerializeField] private EAnimationTarget targetType;
        [SerializeField] private bool loop = true;
        [SerializeField, Min(0f)] private float animationDuration;
        [SerializeField] private bool DestroyOnFinish;
        [SerializeField, ShowAssetPreview] private Sprite[] sprites;
        [SerializeField, ReadOnly] private float timer;
        [SerializeField, ReadOnly] private int currentSpriteIndex;
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
            currentSpriteIndex++;
            if(currentSpriteIndex >= sprites.Length)
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
                        currentSpriteIndex = 0;
                    }
                    else
                    {
                        currentSpriteIndex = sprites.Length - 1; 
                        pause = true;
                    }
                }
            }
            if (targetType == EAnimationTarget.Image)
            {
                image.sprite = sprites[currentSpriteIndex];
            }
            else
            {
                spriteRenderer.sprite = sprites[currentSpriteIndex];
            }

        }
    }
}

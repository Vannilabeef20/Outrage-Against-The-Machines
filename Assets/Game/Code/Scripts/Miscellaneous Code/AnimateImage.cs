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
        [SerializeField, Min(0f)] private float animationDuration;
        [SerializeField] private Sprite[] sprites;
        [SerializeField, ReadOnly] private float timer;
        [SerializeField, ReadOnly] private int currentImageIndex;


        private void Start()
        {
            StartCoroutine(animationRoutine());
        }
        private void Update()
        {
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
                currentImageIndex = 0;
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

        private IEnumerator animationRoutine()
        {
            float frameDuration = animationDuration / sprites.Length;
            if(targetType == EAnimationTarget.Image)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    yield return new WaitForSeconds(frameDuration);
                    image.sprite = sprites[i];
                }
            }
            else
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    yield return new WaitForSeconds(frameDuration);
                    spriteRenderer.sprite = sprites[i];
                }
            }
        }
    }
}

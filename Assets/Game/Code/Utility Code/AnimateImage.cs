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

        [SerializeField, ShowIf("targetType", EAnimationTarget.SpriteRenderer)] SpriteRenderer spriteRenderer;
        [SerializeField, ShowIf("targetType", EAnimationTarget.Image)] Image image;
        [SerializeField] EAnimationTarget targetType;

        [SerializeField] bool playOnAwake = true;
        [SerializeField] bool cull = true;
        [SerializeField] bool loop = true;
        [SerializeField] bool DestroyOnFinish;
        [SerializeField] bool startOffset;
        [SerializeField, Range(0,100), ShowIf("startOffset")] float startOffsetPercent;
        [SerializeField, Min(0f)] float animationDuration;

        [SerializeField, ShowAssetPreview] Sprite[] sprites;
        [Space]
        [SerializeField, ReadOnly] bool playing;
        [SerializeField ,ReadOnly] bool pause;
        [SerializeField, ReadOnly] float timer;
        [SerializeField, ReadOnly] int currentSpriteIndex;
        float FrameDuration => animationDuration / sprites.Length;
        private void Awake()
        {
            if (playOnAwake) playing = true;
            if(startOffset)
            {
                currentSpriteIndex = Mathf.RoundToInt(startOffsetPercent.Map(0, 100, 0, sprites.Length));
            }
        }
        private void Update()
        {
            if (!playing) return;
            if (pause) return;

            switch(targetType)
            { 
                case EAnimationTarget.SpriteRenderer:
                    if (!spriteRenderer.enabled) return;
                    if (!spriteRenderer.isVisible && cull) return;
                    break;
                case EAnimationTarget.Image:
                    if (!image.enabled) return;
                    break;

            }

            timer += Time.deltaTime;

            if (timer < FrameDuration) return;

            timer = 0;
            currentSpriteIndex++;
            if (currentSpriteIndex >= sprites.Length)
            {
                if (DestroyOnFinish)
                {
                    Destroy(gameObject);
                    return;
                }
                if (loop)
                {
                    currentSpriteIndex = 0;
                }
                else
                {
                    currentSpriteIndex = sprites.Length - 1;
                    Stop();
                    return;
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

        [Button("PLAY", EButtonEnableMode.Playmode)]
        public void Play()
        {
            playing = true;
        }

        [Button("STOP", EButtonEnableMode.Playmode)]
        public void Stop()
        {
            playing = false;
        }

        [Button("RESTART", EButtonEnableMode.Playmode)]
        public void Restart()
        {
            timer = 0;
            currentSpriteIndex = 0;
            switch (targetType)
            {
                case EAnimationTarget.Image: image.sprite = null; break;
                case EAnimationTarget.SpriteRenderer: spriteRenderer.sprite = null; break;
            }
            playing = true;
        }

        [Button("PAUSE", EButtonEnableMode.Playmode)]
        public void Pause()
        {
            pause = true;
        }

        [Button("UNPAUSE", EButtonEnableMode.Playmode)]
        public void Unpause()
        {
            pause = false;
        }
    }
}

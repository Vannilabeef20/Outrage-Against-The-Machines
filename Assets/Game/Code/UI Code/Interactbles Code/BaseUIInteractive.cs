using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using DG.Tweening;

namespace Game
{
    public class BaseUIInteractive : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {

        [SerializeField] UIAnimationSO animationSO;
        Coroutine animationRoutine;
        public void AnimateInteractible(AnimationCurve curve, float duration, bool loop = false)
        {
            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animationRoutine = StartCoroutine(AnimationRoutine(curve, duration, loop));
        }

        public IEnumerator AnimationRoutine(AnimationCurve curve, float duration, bool loop = false)
        {
            float timer = 0f;
            while(timer <= duration)
            {
                timer += Time.unscaledDeltaTime;
                transform.localScale = curve.Evaluate(timer.Map(0, duration)) * Vector3.one;
                yield return null;
            }

            if (loop) StartCoroutine(AnimationRoutine(curve, duration, loop));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AnimateInteractible(animationSO.SelectCurve, animationSO.SelectDuration);
        }

        public void OnSelect(BaseEventData eventData)
        {
            AudioManager.instance.PlayUiSelectSfx();
            AnimateInteractible(animationSO.SelectCurve, animationSO.SelectDuration);
        }
        public void PlayInteractionAnimation()
        {
            AnimateInteractible(animationSO.ClickCurve, animationSO.ClickDuration);
        }
    }
}

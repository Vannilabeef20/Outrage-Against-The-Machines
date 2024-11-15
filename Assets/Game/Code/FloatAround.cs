using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace Game
{
	public class FloatAround : MonoBehaviour
	{
        [SerializeField, ShowIf("isRect")] RectTransform rect;
        [SerializeField] bool isRect;
        [SerializeField] AnimationCurve curveX;
        [SerializeField] AnimationCurve curveY;
        [SerializeField] float waveXDuration;
        [SerializeField] float waveYDuration;
        [SerializeField] float distanceX;
        [SerializeField] float distanceY;
        [SerializeField, ReadOnly] Vector3 initialPos;
        [SerializeField, ReadOnly] float timerX;
        [SerializeField, ReadOnly] float timerY;
        [SerializeField, ReadOnly, Range(0, 1)] float progressX;
        [SerializeField, ReadOnly, Range(0, 1)] float progressY;
        [SerializeField, ReadOnly, Range(-1, 1)] float sampleX;
        [SerializeField, ReadOnly, Range(-1, 1)] float sampleY;
        [SerializeField, ReadOnly] Vector2 displacement = Vector2.zero;

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            if (isRect)
                initialPos = rect.anchoredPosition;
            else
                initialPos = transform.position;
        }

        private void Update()
        {
            timerX += Time.deltaTime;
            timerY += Time.deltaTime;

            if (timerX > waveXDuration) timerX = 0;
            if (timerY > waveYDuration) timerY = 0;

            progressX = timerX.Map(0, waveXDuration);
            progressY = timerY.Map(0, waveYDuration);

            sampleX = curveX.Evaluate(progressX);
            sampleY = curveY.Evaluate(progressY);

            displacement.x = sampleX * distanceX;
            displacement.y = sampleY * distanceY;

            if(isRect)
                rect.anchoredPosition = initialPos + new Vector3(displacement.x, displacement.y);
            else
                transform.position = initialPos + new Vector3(displacement.x, displacement.y);
        }
    }
}
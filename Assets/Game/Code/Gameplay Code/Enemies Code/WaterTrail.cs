using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class WaterTrail : MonoBehaviour
	{
		[SerializeField] SpriteRenderer waterRenderer;
		[SerializeField] float fadeInLenght;
		[SerializeField] float fadeOutDelay;
		[SerializeField] float fadeOutLenght;
        [SerializeField, ReadOnly] float timer;

        float totalDuration => fadeInLenght + fadeOutDelay + fadeOutLenght;
        Color color => waterRenderer.color;

        private void Update()
        {
            timer += Time.deltaTime;
            //Fade In

            waterRenderer.color = color.ChangeAlpha(timer.Map(0, fadeInLenght));

            if (timer < fadeInLenght + fadeOutDelay) return;
            //Fade Out

            waterRenderer.color = color.ChangeAlpha(1 - timer.Map(fadeInLenght + fadeOutDelay, totalDuration));

            if(timer > totalDuration)
            {
                Destroy(gameObject);
            }
        }
    }
}
using Game;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    [SerializeField, Required] SpriteRenderer spriteRenderer;
    [SerializeField] bool playOnStart;
    [SerializeField] bool loop;
    [SerializeField, Min(0)] float totalDuration;
    [SerializeField, Min(0)] AnimationCurve colorDurationCurve;
    [SerializeField, ColorUsage(true, true)] Color[] colors;
    [SerializeField, ColorUsage(true, true), ReadOnly] Color originalColor;
    [SerializeField, ReadOnly] int currentColorIndex;
    [SerializeField, ReadOnly] float totalTime;
    [SerializeField, ReadOnly] float colorTime;

    IEnumerator currentRoutine;

    private void Start()
    {
        originalColor = spriteRenderer.color;
        if(playOnStart) Play();
    }

    public void Play()
    {
        Stop();
        StartCoroutine(FlickerRoutine());
    }

    public void Stop()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        spriteRenderer.color = originalColor;
    }

    IEnumerator FlickerRoutine()
    {
        //Setup
        originalColor = spriteRenderer.color;
        currentColorIndex = 0;
        totalTime = 0;
        bool stop = false;
        float colorDuration;
        float normalizedTime = 0;

        //Flicker Loop
        while (stop == false)
        {
            yield return null;

            totalTime += Time.deltaTime;
            colorTime += Time.deltaTime;

            normalizedTime = totalTime.Map(0, totalDuration);
            colorDuration = colorDurationCurve.Evaluate(normalizedTime);

            Debug.Log(colorDuration);

            //Set new color
            if(colorTime > colorDuration)
            {
                currentColorIndex = Helper.Wrap(currentColorIndex + 1, colors.Length - 1);

                spriteRenderer.color = colors[currentColorIndex];

                colorTime = colorTime.Wrap(colorDuration);
            }

            if(totalTime > totalDuration)
            {
                if(loop)
                {
                    totalTime = totalTime.Wrap(totalDuration);
                }
                else
                {
                    stop = true;
                }
            }
        }
        
        spriteRenderer.color = originalColor;
    }
}

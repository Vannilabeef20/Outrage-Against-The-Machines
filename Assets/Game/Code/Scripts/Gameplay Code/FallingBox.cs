using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class FallingBox : MonoBehaviour
	{
        [SerializeField] private SpriteRenderer boxRenderer;
        [SerializeField] private SpriteRenderer highlightRenderer;
        [SerializeField] private Transform boxTransform;
        [SerializeField] private Transform shadowTransform;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip impactSound;

        [SerializeField] private AnimateImage animateImage;
		[SerializeField] private FallingBoxSpeedSO fallingSpeedSO;
        [SerializeField, ReadOnly] private bool fell;
        [SerializeField, ReadOnly] private Vector3 startPos;
        [SerializeField, ReadOnly] private Vector3 finalPos;
        [SerializeField] private int sortOrder = -2;
        [SerializeField, ReadOnly] private Color baseColor;
        [SerializeField] private Color fadeColor;
        [SerializeField, ReadOnly] private float fadeTimer;
        [SerializeField] private float fadeLenght;
        [SerializeField] private float fadeDelay;
        [SerializeField] private Color highlightColor1;
        [SerializeField] private Color highlightColor2;
        [SerializeField, ReadOnly] private float highlightTimer;
        [SerializeField] private float highlightLenght;

        private void Awake()
        {
            fadeTimer = -fadeDelay;
            baseColor = boxRenderer.color;
            highlightRenderer.color = highlightColor1;
            startPos = boxTransform.position;
            finalPos = new Vector3(boxTransform.position.x, boxTransform.position.z, boxTransform.position.z);
            shadowTransform.position = finalPos;
            animateImage.pause = true;
        }
        private void Update()
        {
            if(Time.deltaTime == 0 || fell)
            {
                source.Pause();
            }
            else
            {
                source.UnPause();
            }
            highlightTimer += Time.deltaTime;
            if(highlightTimer > highlightLenght)
            {
                if(highlightRenderer.color == highlightColor1)
                {
                    highlightRenderer.color = highlightColor2;
                }
                else
                {
                    highlightRenderer.color = highlightColor1;
                }
                highlightTimer = 0;
            }

            if(fell)
            {
                fadeTimer += Time.deltaTime;
                fadeDelay += Time.deltaTime;
                if(fadeTimer > fadeLenght) 
                {
                    if(boxRenderer.color == baseColor)
                    {
                        boxRenderer.color = fadeColor;
                    }
                    else
                    {
                        boxRenderer.color = baseColor;
                    }
                    fadeTimer = 0;
                }
            }

            if (!fell)
            {
                shadowTransform.localScale = boxTransform.position.magnitude.Map(startPos.magnitude, finalPos.magnitude) * Vector3.one;
                boxTransform.position += Vector3.down * fallingSpeedSO.FallingSpeed * Time.deltaTime;
            }
            if (boxTransform.position.y < boxTransform.position.z && fell == false)
            {
                boxCollider.enabled = false;
                animateImage.pause = false;
                boxTransform.position = new Vector3(boxTransform.position.x, boxTransform.position.z, boxTransform.position.z);
                fell = false;
                source.Stop();
                source.PlayOneShot(impactSound);
                shadowTransform.localScale = Vector3.zero;
                boxRenderer.sortingOrder = sortOrder;
                Destroy(gameObject, 2f);
                fell = true;
            }
        }
    }
}
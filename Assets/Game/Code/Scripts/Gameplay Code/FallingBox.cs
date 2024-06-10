using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class FallingBox : MonoBehaviour
	{
        [SerializeField] private SpriteRenderer render;
        [SerializeField] private Transform boxTransform;
        [SerializeField] private Transform shadowTransform;
		[SerializeField] private AnimateImage animateImage;
        [SerializeField] private BoxCollider boxCollider;
		[SerializeField] private FallingBoxSpeedSO fallingSpeedSO;
        [SerializeField] private bool fell;
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 finalPos;
        [SerializeField] private int sortOrder = -2;
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip impactSound;
        [SerializeField] private AudioClip fallingSound;
        private void Awake()
        {
            startPos = boxTransform.position;
            finalPos = new Vector3(boxTransform.position.x, boxTransform.position.z, boxTransform.position.z);
            shadowTransform.position = finalPos;
            animateImage.pause = true;
        }
        private void Start()
        {
            source.PlayOneShot(fallingSound);
        }
        private void Update()
        {
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
                render.sortingOrder = sortOrder;
                Destroy(gameObject, 2f);
                fell = true;
            }
        }
    }
}
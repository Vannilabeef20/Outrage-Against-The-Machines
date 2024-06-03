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
		[SerializeField] private float fallingSpeed;
        [SerializeField] private bool falling = true;
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 finalPos;
        [SerializeField] private int sortOrder = -2;
        private void Awake()
        {
            startPos = boxTransform.position;
            finalPos = new Vector3(boxTransform.position.x, boxTransform.position.z, boxTransform.position.z);
            shadowTransform.position = finalPos;
            animateImage.pause = true;
        }
        private void Update()
        {
            if (falling)
            {
                shadowTransform.localScale = boxTransform.position.magnitude.Map(startPos.magnitude, finalPos.magnitude) * Vector3.one;
                boxTransform.position += Vector3.down * fallingSpeed * Time.deltaTime;
            }
            if (boxTransform.position.y < boxTransform.position.z)
            {
                boxCollider.enabled = false;
                animateImage.pause = false;
                boxTransform.position = new Vector3(boxTransform.position.x, boxTransform.position.z, boxTransform.position.z);
                falling = false;
                shadowTransform.localScale = Vector3.zero;
                render.sortingOrder = sortOrder;
                Destroy(gameObject, 2f);
            }
        }
    }
}
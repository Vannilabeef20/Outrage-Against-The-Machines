using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Game
{
	public class ScrollImage : MonoBehaviour
	{
        [SerializeField] private RawImage scrollImage;
        [SerializeField] private Vector2 scrollSpeed;
        [SerializeField, ReadOnly] private Rect tempRect;

        private void Update()
        {
            tempRect = new Rect(scrollImage.uvRect.x + Time.deltaTime * scrollSpeed.x,
                scrollImage.uvRect.y + Time.deltaTime * scrollSpeed.y, scrollImage.uvRect.width,
                scrollImage.uvRect.height);
            scrollImage.uvRect = tempRect;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;


namespace Game
{
    public class ScrapDrop : MonoBehaviour
    {
        [SerializeField, Required] Transform shadowTransform;
        [SerializeField, Required] Rigidbody rb;
        [SerializeField, Required] StudioEventEmitter pickEmitter;
        [SerializeField] LayerMask groundMask;

        [SerializeField, Min(0)] int scrapValue;

        RaycastHit hit;

        private void FixedUpdate()
        {
            Physics.Raycast(transform.position, -transform.up, out hit, float.MaxValue, groundMask);

            shadowTransform.position = hit.point;
        }

        private void OnTriggerEnter(Collider other)
        {
            for(int i = 0; i < GameManager.Instance.PlayerTags.Length; i++)
            {
                if (other.gameObject.CompareTag(GameManager.Instance.PlayerTags[i]))
                {
                    pickEmitter.Play();
                    GameManager.Instance.PlayerCharacterList[i].scrapAmount += scrapValue;
                    Destroy(transform.parent.gameObject);
                    break;
                }
            }           
        }
    }

}
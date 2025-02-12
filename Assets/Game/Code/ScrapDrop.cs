using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;


namespace Game
{
    public class ScrapDrop : MonoBehaviour
    {

        #region REFERENCES
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] Transform shadowTransform;
        [SerializeField] Rigidbody rb;
        [SerializeField] StudioEventEmitter pickEmitter;

        #endregion

        #region PARAMETERS
        [Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField, Min(0)] int scrapValue;

        [SerializeField] float gravity = -0.1f;

        [SerializeField, Range(0, 360)] float gravRot;

        [SerializeField, Min(0)] int bounces = 2;

        [SerializeField, Min(0)] float bounceDecay;

        [SerializeField, MinMaxSlider(0f, 99f)] Vector2 forceRange;
        #endregion

        #region VARIABLES
        [Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField, ReadOnly] float force;

        [SerializeField, ReadOnly] int bounceCount;

        #endregion
        private void FixedUpdate()
        {
            shadowTransform.position = transform.position.ToXZZ();

            if (transform.position.y > transform.position.z)
            {
                rb.linearVelocity += gravity * (Quaternion.AngleAxis(gravRot, Vector3.right) * Vector3.up);
            }

            if (transform.position.y < transform.position.z)
            {
                if (bounceCount >= bounces)
                {
                    rb.linearVelocity = Vector3.zero;
                    transform.position = transform.position.ToXYY();
                }
                else
                {
                    bounceCount++;
                    float aaa = force / ((bounceCount + 1) * bounceDecay);
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(aaa * Vector3.up, ForceMode.Impulse);
                }
            }
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

        public void ApplyForce(Vector3 dir)
        {
            force = Random.Range(forceRange.x, forceRange.y);
            rb.AddForce(force * dir, ForceMode.Impulse);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, gravity * (Quaternion.AngleAxis(gravRot, Vector3.right) * Vector3.up) * 5);
        }
#endif
    }

}
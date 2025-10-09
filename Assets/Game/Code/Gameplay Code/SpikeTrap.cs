using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;
using FMODUnity;

namespace Game
{
    [SelectionBase]
    public class SpikeTrap : MonoBehaviour
    {
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] Animator animator;
        [SerializeField] AnimationClip spikeAnimation;
        [SerializeField, ReadOnly] Transform followCamTransform;

        [Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField] float activationRange;
        [SerializeField, Min(0.01f)] float animationDuration = 1;
        [SerializeField] AnimationFrameEvent[] frameEvents;

        [Header("DAMAGE")]
        [SerializeField] LayerMask playerMask;
        [SerializeField] LayerMask enemyMask;

        [SerializeField] float damage;

        [Tooltip("How long (seconds) the entity will be stunned after taking damage.")]
        [SerializeField] float stunDuration;

        [Tooltip("How strong the force applied to the entity will be.")]
        [SerializeField] float knockbackStrenght;

        [Tooltip("Value to be multiplied to damage value if an enemy is damaged instead of a player.")]
        [SerializeField] float enemyMultiplier;


        [Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, ReadOnly] float timer;
        [SerializeField, ReadOnly] List<Collider> hitList;

        private void Awake()
        {
            followCamTransform = Camera.main.transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().transform;
            foreach (var frameEvent in frameEvents)
            {
                frameEvent.Setup(spikeAnimation, animationDuration);
            }
        }
        private void Update()
        {
            //Return and pause animation if not within the activation range
            if (Mathf.Abs(followCamTransform.position.x - transform.position.x) > activationRange)
            {
                animator.speed = 0;
                return;
            }

            animator.speed = spikeAnimation.length/animationDuration;

            timer += Time.deltaTime;
            if (timer > animationDuration) timer -= animationDuration;

            animator.Play(spikeAnimation.name, 0, timer.Map(0, animationDuration));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hitList.Contains(other)) return;
            hitList.Add(other);
            if (playerMask.ContainsLayer(other.gameObject.layer))
            {
                if (other.TryGetComponent<IDamageble>(out IDamageble damageble))
                {
                    damageble.TakeDamage(transform.position, damage, stunDuration, knockbackStrenght);
                }
            }
            else if (enemyMask.ContainsLayer(other.gameObject.layer))
            {
                if (other.TryGetComponent<IDamageble>(out IDamageble damageble))
                {
                    damageble.TakeDamage(transform.position, damage * enemyMultiplier, stunDuration, knockbackStrenght);
                }
            }
        }

        public void ClearHitArray()
        {
            hitList.Clear();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            #region DRAW ACTIVATION RANGE ZONE
            Vector3 point1Pos = Vector3.zero;
            Vector3 point2Pos = Vector3.zero;

            point1Pos.x = transform.position.x - activationRange;
            point2Pos.x = transform.position.x - activationRange;
            point1Pos.y = transform.position.y + 7;
            point2Pos.y = transform.position.y - 7;
            point1Pos.z = transform.position.z;
            point2Pos.z = transform.position.z;
            Debug.DrawLine(point1Pos, point2Pos, Color.yellow); //Activation range Left Line


            point1Pos.x = transform.position.x + activationRange;
            point2Pos.x = transform.position.x + activationRange;
            point1Pos.y = transform.position.y + 7;
            point2Pos.y = transform.position.y - 7;
            point1Pos.z = transform.position.z;
            point2Pos.z = transform.position.z;
            Debug.DrawLine(point1Pos, point2Pos, Color.yellow); //Activation range RightLine

            point1Pos = transform.position;
            point2Pos = transform.position;
            point1Pos.x = transform.position.x + activationRange;
            point2Pos.x = transform.position.x - activationRange;
            Debug.DrawLine(point1Pos, point2Pos, Color.green); //Activation range MiddleLine
            #endregion
        }
#endif
    }
}


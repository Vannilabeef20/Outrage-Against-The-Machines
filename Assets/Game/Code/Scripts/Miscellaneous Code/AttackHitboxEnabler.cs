using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AttackHitboxEnabler : MonoBehaviour
    {
        [SerializeField] private BoxCollider[] hitboxes;
        private void OnEnable()
        {
            DisableAttackbox();
        }

        private void EnableAttackbox()
        {
            foreach (BoxCollider box in hitboxes)
            {
                box.enabled = true;
            }
        }

        private void DisableAttackbox()
        {
            foreach (BoxCollider box in hitboxes)
            {
                box.enabled = false;
            }
        }
    }
}

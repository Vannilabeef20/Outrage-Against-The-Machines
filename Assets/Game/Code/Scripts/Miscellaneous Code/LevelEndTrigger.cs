using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class LevelEndTrigger : MonoBehaviour
    {
        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private IntEvent LoadSceneEvent;
        [SerializeField, Scene] private int nextScene;
        private void OnTriggerEnter(Collider other)
        {
            if(playerLayerMask.ContainsLayer(other.gameObject.layer))
            {
                LoadSceneEvent.Raise(this, nextScene);
            }
        }
    }
}

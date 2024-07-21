using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// OnTriggerEnter Triggers a loading transition.
    /// </summary>
    public class LevelEndTrigger : MonoBehaviour
    {
        [Tooltip("Layers that will be able to trigger the transition." +
            "\nAKA just the player physical collider layer.")]
        [SerializeField] LayerMask playerLayerMask;
        [Tooltip("The name of the next scene.")]
        [SerializeField, Scene] int nextScene;

        private void Start()
        {
            PlayerPrefs.SetInt("IsTutorialCompleted", 1);
        }
        private void OnTriggerEnter(Collider other)
        {
            if(playerLayerMask.ContainsLayer(other.gameObject.layer))
            {
                GameManager.Instance.LoadScene(nextScene);
                PlayerPrefs.SetInt("IsTutorialCompleted", 1);
            }
        }
    }
}

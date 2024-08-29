using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// OnTriggerEnter Triggers a loading transition.
    /// <br/>[Workaround] Also sets tutorial completion.
    /// </summary>
    public class LevelEndTrigger : MonoBehaviour
    {
        [Header("PARAMETERS"), HorizontalLine(2F, EColor.Red)]
        [Tooltip("Layers that will be able to trigger the transition.")]
        [SerializeField] LayerMask playerLayerMask;
        [Tooltip("The name of the next scene.")]
        [SerializeField, Scene] int nextScene;

        private void Start()
        {
            PlayerPrefs.SetInt("IsTutorialCompleted", 1);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (playerLayerMask.ContainsLayer(other.gameObject.layer))
            {
                TransitionManager.Instance.LoadScene(nextScene);
                PlayerPrefs.SetInt("IsTutorialCompleted", 1);
            }
        }
    }
}

using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [CreateAssetMenu(fileName = "Transition", menuName = "Misc/New Transtion")]
    public class TransitionSO : ScriptableObject
    {
       [field: SerializeField] public float Duration { get; private set; }
       [field: SerializeField, ShowAssetPreview] public Sprite[] Sprites { get; private set; }
    }
}

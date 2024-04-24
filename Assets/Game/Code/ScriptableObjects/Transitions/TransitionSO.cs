using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Transition", menuName = "New Transtion")]
    public class TransitionSO : ScriptableObject
    {
       [field: SerializeField] public float Duration { get; private set; }
       [field: SerializeField] public Sprite[] Sprites { get; private set; }
    }
}

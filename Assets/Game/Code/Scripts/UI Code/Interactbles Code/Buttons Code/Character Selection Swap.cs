using UnityEngine;


namespace Game 
{
    public class CharacterSelectionSwap : MonoBehaviour
    {
        [SerializeField] private CharacterSelector characterSelector;
        [SerializeField] private int characterIndex;
        [SerializeField] private int cycleValue;

        public void Swap()
        {
            characterSelector.SwapCharacter(characterIndex, cycleValue);
        }
    }
}

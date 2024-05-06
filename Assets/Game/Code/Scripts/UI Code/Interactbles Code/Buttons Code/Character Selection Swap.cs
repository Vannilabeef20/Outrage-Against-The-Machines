using UnityEngine;


namespace Game 
{
    public class CharacterSelectionSwap : MonoBehaviour
    {
        [SerializeField] private CharacterSelector characterSelector;
        [SerializeField] private int characterIndex;

        public void Swap()
        {
            characterSelector.SwapCharacter(characterIndex);
        }
    }
}

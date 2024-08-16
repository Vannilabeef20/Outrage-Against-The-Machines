using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public abstract class BaseInteractble : MonoBehaviour
	{
		[SerializeField] SpriteRenderer interactionPromptRenderer;

        [SerializeField] SpriteRenderer player1NumberRenderer;
        [SerializeField] SpriteRenderer player2NumberRenderer;
        [SerializeField] SpriteRenderer player3NumberRenderer;
        private void Awake()
        {
            interactionPromptRenderer.enabled = false;
            player1NumberRenderer.enabled = false;
            player2NumberRenderer.enabled = false;
            player3NumberRenderer.enabled = false;
        }
        public abstract void Interact(int playerNumber);



        public void UpdateSelection(int playerNumber, bool enable)
        {
            switch(playerNumber)
            {
                case 0: player1NumberRenderer.enabled = enable; break;
                case 1: player2NumberRenderer.enabled = enable; break;
                case 2: player3NumberRenderer.enabled = enable; break;
            }

            switch(player1NumberRenderer.enabled, player2NumberRenderer.enabled, player3NumberRenderer.enabled)

            {
                case (false, false, false): interactionPromptRenderer.enabled = false; break;

                default : interactionPromptRenderer.enabled = true; break;
            }
        }

    }
}
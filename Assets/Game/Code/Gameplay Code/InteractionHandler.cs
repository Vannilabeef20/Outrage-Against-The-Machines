using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
	public class InteractionHandler : MonoBehaviour
	{
        #region REFERENCES
        [field: Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] GameObject interactPromptController;
        [SerializeField] GameObject interactPromptKeyboard;
        [field: SerializeField] public PlayerInput playerInput { get; private set; }
        [SerializeField] BoxCollider detectionShape;
        [SerializeField] StudioEventEmitter sucessfullEmitter;
        #endregion

        #region PARAMETERS & VARIABLES
        [Header("PARAMETERS & VARIABLES"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] LayerMask interactionMask;
        [SerializeField, ReadOnly] Collider[] interactblesCollidersInRange;
        [SerializeField, ReadOnly] Collider closestInteractbleCollider;
        [SerializeField, ReadOnly] BaseInteractble closestInteractble;
        [field: SerializeField, ReadOnly] public bool InRange { get; private set; }
        #endregion

        private void FixedUpdate()
        {
            interactPromptController.SetActive(InRange && playerInput.currentControlScheme == "Gamepad");
            interactPromptKeyboard.SetActive(InRange && playerInput.currentControlScheme == "Keyboard");
            InteractionDetection();
        }

        void InteractionDetection() 
        {
            //Detect and Sort all available interactbles in range by distance
            interactblesCollidersInRange = Physics.OverlapBox(detectionShape.bounds.center,
                detectionShape.size * 0.5f, Quaternion.identity, interactionMask).OrderBy
                (interactible => (detectionShape.bounds.center - interactible.bounds.center).magnitude).ToArray();

            InRange = interactblesCollidersInRange.Length > 0;

            //No interactbles available
            if (!InRange)
            {
                closestInteractbleCollider = null;
                if (closestInteractble != null)
                {
                    closestInteractble.UpdateSelection(playerInput.playerIndex, false);
                    closestInteractble = null;
                }
                return;
            }

            if (closestInteractbleCollider == null)
            {
                closestInteractbleCollider = interactblesCollidersInRange[0];
            }
            else if (interactblesCollidersInRange[0] == closestInteractbleCollider) return;

            //Deactivate former closestInteractible player prompt
            if (closestInteractble != null)
            {
                closestInteractble.UpdateSelection(playerInput.playerIndex, false);
            }
            closestInteractbleCollider = interactblesCollidersInRange[0];
            //Update new closestInteractible
            closestInteractble = closestInteractbleCollider.GetComponent<BaseInteractble>();
            closestInteractble.UpdateSelection(playerInput.playerIndex, true);
        }

        public void RequestInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (Time.deltaTime <= 0) return;

            if (closestInteractble == null) return;

            closestInteractble.Interact(playerInput.playerIndex);
        }
    }
}
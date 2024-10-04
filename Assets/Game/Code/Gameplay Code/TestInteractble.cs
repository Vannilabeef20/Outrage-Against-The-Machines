using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class TestInteractble : BaseInteractble
    {
        public override void Interact(int playerNumber)
        {
            string status;
            string costString;

            if (costsMoney)
            {
                costString = $"${costAmount}";
                //Has money
                if (GameManager.Instance.PlayerCharacterList[playerNumber].scrapAmount >= costAmount)
                {
                    status = "Sucessful purchase";
                    GameManager.Instance.PlayerCharacterList[playerNumber].scrapAmount -= costAmount; 
                }
                else //Does not have money
                {
                    status = "Failed purchase";
                }
            }
            else
            {
                status = "Sucessful interaction";
                costString = "No cost";
            }

            this.Log($"Interaction to Base Interactible {name} {status} {costString}");
        }

        public override void OnInteract(int playerNumber) { }
    }
}
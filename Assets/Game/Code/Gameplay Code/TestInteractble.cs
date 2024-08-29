using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class TestInteractble : BaseInteractble
    {
        public override void Interact(int playerNumber)
        {
            if(costsMoney)
            {
                if (GameManager.Instance.PlayerCharacterList[playerNumber].scrapAmount >= costAmount)
                {
                    GameManager.Instance.PlayerCharacterList[playerNumber].scrapAmount -= costAmount;
                    this.Log($"Player {playerNumber} has bought {this.GetType().Name}.");
                }
                else
                {
                    this.Log($"Player {playerNumber} does not have enough money to buy {this.GetType().Name}.");
                }
            }
            else
            {
                this.Log($"Player {playerNumber} has picked {this.GetType().Name}.");
            }
        }
    }
}
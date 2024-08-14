using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public abstract class PlayerState : BaseState
    {
        [Header("PLAYER_STATE INHERITED"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField, ReadOnly] protected PlayerStateMachine stateMachine; 

        public virtual void Setup(PlayerStateMachine playerStateMachine)
        {
            stateMachine = playerStateMachine;
        }
    }
}

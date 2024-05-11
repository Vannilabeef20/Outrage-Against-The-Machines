using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public abstract class PlayerState : BaseState
    {
        [field: Header("#PLAYER STATE# INHERITED"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField, ReadOnly] protected PlayerStateMachine stateMachine; 

        public virtual void Setup(PlayerStateMachine playerStateMachine)
        {
            stateMachine = playerStateMachine;
        }
    }
}

using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public abstract class BossState : BaseState
    {
        [Header("ENEMY_STATE INHERITED"), HorizontalLine(2f,EColor.Orange)]
        [SerializeField] protected BossStateMachine stateMachine;

        public virtual void Setup(BossStateMachine _enemyStateMachine)
        {
            stateMachine = _enemyStateMachine;
        }
    }
}

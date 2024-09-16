using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public abstract class EnemyState : BaseState
    {
        [Header("ENEMY_STATE INHERITED"), HorizontalLine(2f,EColor.Orange)]
        [SerializeField] protected EnemyStateMachine stateMachine;

        public virtual void Setup(EnemyStateMachine _enemyStateMachine)
        {
            stateMachine = _enemyStateMachine;
        }
    }
}

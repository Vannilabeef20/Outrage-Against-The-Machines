using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public abstract class EnemyState : BaseState
    {
        [Header("ENEMY STATE INHERITED"), HorizontalLine(2f,EColor.Orange)]
        [SerializeField, ReadOnly] protected EnemyStateMachine stateMachine;

        public void Setup(EnemyStateMachine _enemyStateMachine)
        {
            stateMachine = _enemyStateMachine;
        }
    }
}

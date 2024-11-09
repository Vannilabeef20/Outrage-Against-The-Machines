using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public abstract class EnemyState : BaseState
    {
        [Header("ENEMY_STATE INHERITED"), HorizontalLine(2f,EColor.Orange)]
        [SerializeField] protected EnemyStateMachine stateMachine;

        protected GameObject Parent { get => stateMachine.Parent; }

        protected Vector3 MachinePosition { get => stateMachine.transform.position; set { stateMachine.transform.position = value; } }
        protected Quaternion MachineRotation { get => stateMachine.transform.rotation; set { stateMachine.transform.rotation = value; } }

        protected Animator MachineAnimator { get => stateMachine.animator; private set { } }

        protected Vector3 Velocity { get => stateMachine.body.velocity; set { stateMachine.body.velocity = value; } }
        protected Vector3 BodyPosition { get => stateMachine.body.position; set { stateMachine.body.position = value; } }

        protected EnemyState NextState { get => stateMachine.nextState; set { stateMachine.nextState = value; } }

        protected Color SpriteColor { get => stateMachine.spriteRenderer.color; set { stateMachine.spriteRenderer.color = value; } }


        public virtual void Setup(EnemyStateMachine _enemyStateMachine)
        {
            stateMachine = _enemyStateMachine;
        }
    }
}

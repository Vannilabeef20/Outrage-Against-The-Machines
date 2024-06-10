using UnityEngine.UI;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyHealthHandler : MonoBehaviour, IDamageble
    {
        [SerializeField, TextArea] private string Comment;
        [SerializeField] private Image healthImage;
        [SerializeField, ReadOnly] private EnemyStateMachine stateMachine;

        #region Enemy Health Params
        [Header("ENEMY HEALTH"), HorizontalLine(2f, EColor.Red)]
        [Tooltip("These are all the collison layers that can damage this entity")]
        [SerializeField] private LayerMask hostileLayers;

        [Tooltip("This enemy's max/initial health points")]
        [SerializeField] private float maxHeathPoints;

        [Tooltip("This enemy's current health points, a number <= 0 triggers death")]
        [field: SerializeField, ProgressBar("HP", "maxHeathPoints", EColor.Red)] public float CurrentHealthPoints { get; private set; }
        [field: SerializeField, ReadOnly] public float CurrentHealthPercent { get; private set; }

        #endregion

        private void Awake()
        {
            stateMachine = transform.parent.GetComponent<EnemyStateMachine>();
            CurrentHealthPoints = maxHeathPoints;
            CurrentHealthPercent = CurrentHealthPoints / maxHeathPoints;
        }

        public virtual void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            if(CurrentHealthPoints <= 0)
            {
                return;
            }
            CurrentHealthPoints -= damage;
            CurrentHealthPercent =  CurrentHealthPoints / maxHeathPoints;
            healthImage.fillAmount = CurrentHealthPercent;
            stateMachine.TakeDamage(damageDealerPos, stunDuration, knockbackStrenght);
        }

    }
}
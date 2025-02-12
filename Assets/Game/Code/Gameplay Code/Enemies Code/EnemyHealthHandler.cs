using UnityEngine.UI;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;

namespace Game
{
    /// <summary>
    /// Handles enemy health. 
    /// </summary>
    public class EnemyHealthHandler : MonoBehaviour, IDamageble
    {
        [SerializeField] EnemyStateMachine stateMachine;
        [SerializeField] Image DEBUGHealthImage;

        #region Enemy Health Params
        [Header("ENEMY HEALTH"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] float maxHeathPoints;

        [field: SerializeField, ReadOnly] public float CurrentHealthPercent { get; private set; }
        [field: SerializeField, ProgressBar("HP", "maxHeathPoints", EColor.Red)] public float CurrentHealthPoints { get; private set; }

        #endregion

        private void Awake()
        {
            CurrentHealthPoints = maxHeathPoints;
            CurrentHealthPercent = CurrentHealthPoints / maxHeathPoints;
        }

        public virtual void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            if (CurrentHealthPoints <= 0) return;

            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0, maxHeathPoints);
            CurrentHealthPercent = CurrentHealthPoints / maxHeathPoints;
            DEBUGHealthImage.fillAmount = CurrentHealthPercent;
            stateMachine.TakeDamage(damageDealerPos, stunDuration, knockbackStrenght);
        }
    }
}
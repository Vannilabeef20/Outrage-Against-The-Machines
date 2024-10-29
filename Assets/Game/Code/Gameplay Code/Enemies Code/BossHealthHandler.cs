using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using DG.Tweening;

namespace Game
{
    /// <summary>
    /// Handles enemy health. 
    /// </summary>
    public class BossHealthHandler : MonoBehaviour, IDamageble
    {
        [SerializeField] BossStateMachine stateMachine;
        [SerializeField] Image InstantHealthBar;
        [SerializeField] Image LerpHealthBar;

        #region Enemy Health Params
        [Header("ENEMY HEALTH"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] float maxHeathPoints;
        [SerializeField, Expandable] LerpConfigSO lerpConfigSO;
        [field: SerializeField, ReadOnly] public float CurrentHealthPercent { get; private set; }
        [field: SerializeField, ProgressBar("HP", "maxHeathPoints", EColor.Red)] public float CurrentHealthPoints { get; private set; }

        Tween healthLerpTween;
        Coroutine healthCoroutine;
        #endregion

        private void Awake()
        {
            InstantHealthBar.fillAmount = 1f;
            LerpHealthBar.fillAmount = 1f;
            CurrentHealthPoints = maxHeathPoints;
            CurrentHealthPercent = CurrentHealthPoints / maxHeathPoints;
        }

        public virtual void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            if(CurrentHealthPoints <= 0)
            {
                return;
            }
            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0, maxHeathPoints);
            CurrentHealthPercent = CurrentHealthPoints / maxHeathPoints;
            InstantHealthBar.fillAmount = CurrentHealthPercent;
            stateMachine.TakeDamage(damageDealerPos, stunDuration, knockbackStrenght);
            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
            }
            healthCoroutine = StartCoroutine(LerpHealthRoutine(CurrentHealthPercent));
        }


        public IEnumerator LerpHealthRoutine(float newHealthPercent)
        {
            if (healthLerpTween != null) healthLerpTween.Kill();
            yield return new WaitForSeconds(lerpConfigSO.Delay);
            healthLerpTween = LerpHealthBar.DOFillAmount(newHealthPercent,
                lerpConfigSO.Duration).SetEase(lerpConfigSO.Ease);
            healthCoroutine = null;
        }
    }
}
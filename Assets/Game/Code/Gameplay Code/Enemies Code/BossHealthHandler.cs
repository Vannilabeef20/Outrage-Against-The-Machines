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
        [field: Header("ENEMY HEALTH"), HorizontalLine(2f, EColor.Red)]

        [field: SerializeField, ProgressBar("HP", "maxHeathPoints", EColor.Red)]
        public float CurrentHealthPoints { get; private set; }

        [SerializeField] float maxHeathPoints;
        [SerializeField, Expandable] LerpConfigSO lerpConfigSO;
        [field: SerializeField, ReadOnly] public float CurrentHealthPercent { get; private set; }

        Tween healthLerpTween;
        Coroutine healthCoroutine;
        #endregion


        #region Enemy Poise Params
        [Header("ENEMY POISE"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField, ProgressBar("Poise", "MaxPoise", EColor.Yellow)] float currentPoise;

        [SerializeField, Min(0f)] float baseMaxPoise;

        [SerializeField, Min(0f)] float poiseRegen;

        [SerializeField, Min(0f)] float poiseGracePeriod;

        [SerializeField, ReadOnly] bool canTakePoiseDamage = true;

        [SerializeField, ReadOnly] float additiveMaxPoise;
        float MaxPoise => baseMaxPoise + additiveMaxPoise;
        float PoiseRegenPoints => Time.deltaTime * poiseRegen;

        #endregion

        private void Awake()
        {
            currentPoise = MaxPoise;
            InstantHealthBar.fillAmount = 1f;
            LerpHealthBar.fillAmount = 1f;
            CurrentHealthPoints = maxHeathPoints;
            CurrentHealthPercent = CurrentHealthPoints / maxHeathPoints;
        }

        private void Update()
        {
            if(canTakePoiseDamage)
            currentPoise = Mathf.Clamp(currentPoise + PoiseRegenPoints, 0, MaxPoise);
            
            /*
            hitFlashTimer += Time.deltaTime;
            if (hitFlashTimer >= hitFlashLenght)
            {
                if (stateMachine.spriteRenderer.color == startingColor)
                {
                    stateMachine.spriteRenderer.color = hitFlashColor;
                }
                else
                {
                    stateMachine.spriteRenderer.color = startingColor;
                }
                hitFlashTimer = 0f;
            }
            */
        }

        public virtual void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            if (CurrentHealthPoints <= 0)
            {
                return;
            }

            currentPoise = Mathf.Clamp(currentPoise - damage, 0, MaxPoise);
            if (currentPoise <= 0)
            {
                canTakePoiseDamage = false;
                StartCoroutine(PoiseGracePeriodRoutine());
            }

            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0, maxHeathPoints);
            CurrentHealthPercent = CurrentHealthPoints / maxHeathPoints;
            InstantHealthBar.fillAmount = CurrentHealthPercent;

            if(CurrentHealthPoints <= 0)
            {
                stateMachine.Kill();
            }
            else if(canTakePoiseDamage) stateMachine.Stun(damageDealerPos, stunDuration, knockbackStrenght);

            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
            }
            healthCoroutine = StartCoroutine(LerpHealthRoutine(CurrentHealthPercent));
        }

        public void Phase2()
        {
            CurrentHealthPoints = maxHeathPoints;
            CurrentHealthPercent = CurrentHealthPoints / maxHeathPoints;
            InstantHealthBar.fillAmount = CurrentHealthPercent;
            LerpHealthBar.fillAmount = CurrentHealthPercent;
            stateMachine.phase2 = true;
        }


        public IEnumerator LerpHealthRoutine(float newHealthPercent)
        {
            if (healthLerpTween != null) healthLerpTween.Kill();
            yield return new WaitForSeconds(lerpConfigSO.Delay);
            healthLerpTween = LerpHealthBar.DOFillAmount(newHealthPercent,
                lerpConfigSO.Duration).SetEase(lerpConfigSO.Ease);
            healthCoroutine = null;
        }

        public IEnumerator PoiseGracePeriodRoutine()
        {
            yield return new WaitForSeconds(poiseGracePeriod);
            currentPoise = MaxPoise;
            canTakePoiseDamage = true;
        }
    }
}
using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] StudioEventEmitter poiseHitEmitter;

        #region Enemy Health Params
        [Header("ENEMY HEALTH"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] float[] perPlayerMaxHealth;

        [SerializeField, Expandable] LerpConfigSO lerpConfigSO;
        [field: SerializeField] public float CurrentHealthPoints { get; private set; }
        [field: SerializeField, ReadOnly] public float CurrentHealthPercent { get; private set; }

        int PlayerCount => GameManager.Instance.PlayerCharacterList.Count;
        float MaxHeathPoints => perPlayerMaxHealth[PlayerCount - 1];

        Tween healthLerpTween;
        Coroutine healthCoroutine;
        #endregion


        #region Enemy Poise Params
        [Header("ENEMY POISE"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField, ProgressBar("Poise", "MaxPoise", EColor.Yellow)] float currentPoise;

        [SerializeField] Color poiseFlickerColor;

        [SerializeField] Color poiseHitColor;

        [SerializeField, Min(0f)] float baseMaxPoise;

        [SerializeField, Min(0f)] float poiseRegen;

        [SerializeField, Min(0f)] float poiseGracePeriod;

        [SerializeField] float poiseFlickerLenght;

        [SerializeField, ReadOnly] bool canTakePoiseDamage = true;

        [SerializeField, ReadOnly] float poiseFlickerTimer;

        [SerializeField, ReadOnly] float additiveMaxPoise;
        float MaxPoise => baseMaxPoise + additiveMaxPoise;
        float PoiseRegenPoints => Time.deltaTime * poiseRegen;

        #endregion

        private void Start()
        {
            currentPoise = MaxPoise;
            InstantHealthBar.fillAmount = 1f;
            LerpHealthBar.fillAmount = 1f;
            CurrentHealthPoints = MaxHeathPoints;
            CurrentHealthPercent = CurrentHealthPoints / MaxHeathPoints;
        }

        private void Update()
        {
            if(canTakePoiseDamage)
            {
                currentPoise = Mathf.Clamp(currentPoise + PoiseRegenPoints, float.MinValue, MaxPoise);
                stateMachine.spriteRenderer.color = Color.white;
                return;
            }
                        
            poiseFlickerTimer += Time.deltaTime;
            if (poiseFlickerTimer >= poiseFlickerLenght)
            {
                poiseFlickerTimer = 0f;
                if (stateMachine.spriteRenderer.color == poiseFlickerColor)
                {
                    stateMachine.spriteRenderer.color = Color.white;
                }
                else
                {
                    stateMachine.spriteRenderer.color = poiseFlickerColor;
                }
            }                              
        }

        public virtual void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            if (CurrentHealthPoints <= 0)
            {
                return;
            }

            if (currentPoise < 0)
            {
                poiseHitEmitter.Play();
                canTakePoiseDamage = false;
                StartCoroutine(PoiseGracePeriodRoutine());
            }
            currentPoise = Mathf.Clamp(currentPoise - damage, float.MinValue, MaxPoise);

            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0, MaxHeathPoints);
            CurrentHealthPercent = CurrentHealthPoints / MaxHeathPoints;
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

            stateMachine.spriteRenderer.color = poiseHitColor;
        }

        public void Phase2()
        {
            currentPoise = MaxPoise;
            CurrentHealthPoints = MaxHeathPoints;
            CurrentHealthPercent = CurrentHealthPoints / MaxHeathPoints;
            InstantHealthBar.fillAmount = CurrentHealthPercent;
            LerpHealthBar.fillAmount = CurrentHealthPercent;
            stateMachine.SetPhase2();
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
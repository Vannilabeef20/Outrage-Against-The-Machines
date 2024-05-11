using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;

namespace Game
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private int playerID;
        #region Health Params
        [Header("Health"), HorizontalLine(2f, EColor.Red)]
        [SerializeField, ShowAssetPreview] private Sprite[] sprites;
        [SerializeField] private Image foregroundImage;
        [SerializeField] private Image InstantHealthBar;
        [SerializeField] private Image LerpHealthBar;
        [SerializeField, Expandable] private PlayerHealthUIConfigSO healthConfigSO;
        private Tween healthLerpTween;
        private Coroutine healthCoroutine;
        #endregion
        #region Special Params
        [Header("Special Params"), HorizontalLine(2f, EColor.Blue)]
        [SerializeField] private Image InstantSpecialBar;
        [SerializeField] private Image LerpSpecialBar;
        [SerializeField, Expandable] private PlayerSpecialUIConfigSO specialConfigSO;
        private Tween specialLerpTween;
        private Coroutine specialCoroutine;
        #endregion
        private void Start()
        {            
            if (GameManager.Instance.playerIndexes.Count < playerID + 1)
            {
                transform.gameObject.SetActive(false);
            }
        }
        public void UpdateHealthbar(PlayerHitParams playerHitParams)
        {
            if(playerHitParams.playerID != playerID)
            {
                return;
            }
            int index = (int)Mathf.Round(1 - playerHitParams.newHealthPercent.Map(0, 1, 0, sprites.Length - 1));
            index = Mathf.Clamp(index, 0, sprites.Length);
            foregroundImage.sprite = sprites[index];
            if(healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
            }
            healthCoroutine = StartCoroutine(LerpHealthRoutine(playerHitParams));
        }
        private IEnumerator LerpHealthRoutine(PlayerHitParams playerHitParams)
        {
            healthLerpTween?.Kill();
            InstantHealthBar.fillAmount = playerHitParams.newHealthPercent;
            yield return new WaitForSeconds(healthConfigSO.HealthLerpDelay);
            healthLerpTween = LerpHealthBar.DOFillAmount(playerHitParams.newHealthPercent,
                healthConfigSO.HealthLerpDuration).SetEase(healthConfigSO.HealthLerpEase);
            healthCoroutine = null;
        }

        public void UpdateSpecialbar(PlayerSpecialParams playerSpecialParams)
        {
            if (playerSpecialParams.playerID != playerID)
            {
                return;
            }
            if (specialCoroutine != null)
            {
                StopCoroutine(specialCoroutine);
            }
            specialCoroutine = StartCoroutine(LerpSpecialRoutine(playerSpecialParams));
        }
        private IEnumerator LerpSpecialRoutine(PlayerSpecialParams playerSpecialParams)
        {
            specialLerpTween?.Kill();
            InstantSpecialBar.fillAmount = playerSpecialParams.newSpecialPercent;
            yield return new WaitForSeconds(specialConfigSO.SpecialLerpDelay);
            specialLerpTween = LerpSpecialBar.DOFillAmount(playerSpecialParams.newSpecialPercent,
                specialConfigSO.SpecialLerpDuration).SetEase(specialConfigSO.SpecialLerpEasing);
            specialCoroutine = null;
        }

        public void UpdateDeathStatus(PlayerDeathParams playerDeathParams)
        {
            if(playerDeathParams.playerID != playerID)
            {
                return;
            }
            gameObject.SetActive(!playerDeathParams.isPlayerDead);
        }
    }
}
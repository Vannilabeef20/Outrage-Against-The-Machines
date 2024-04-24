using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private int playerID;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Image foregroundImage;
        [SerializeField] private Image InstantHealthBar;
        [SerializeField] private Image LerpHealthBar;
        private Tween healthLerpTween;
        private Coroutine healthCoroutine;

        private void Start()
        {
            
            if (GameManager.Instance.playerIndexes[playerID] < 0)
            {
                transform.parent.gameObject.SetActive(false);
            }
        }
        public void UpdateHealthbar(PlayerHitParams playerHitParams)
        {
            if(playerHitParams.playerID != playerID)
            {
                return;
            }
            int index = (int)Mathf.Round(playerHitParams.newHealthPercent.Map(1, 0, 0, sprites.Length - 1, true));
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
            yield return new WaitForSeconds(playerHitParams.healthLerpDelay);
            healthLerpTween = LerpHealthBar.DOFillAmount(playerHitParams.newHealthPercent,
                playerHitParams.healthLerpDuration).SetEase(playerHitParams.healthLerpEasing);
            healthCoroutine = null;
        }
        
    }
}
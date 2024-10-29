
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using DG.Tweening;

namespace Game
{
	public class PlayerGUI : MonoBehaviour
	{
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] Image playerIcon;
        [SerializeField] Image itemIcon;
        [SerializeField] Image InstantHealthBar;
        [SerializeField] Image LerpHealthBar;
        [SerializeField] TextMeshProUGUI scrapCountText;
        [SerializeField] Image[] specialImages;

        [Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] int playerNumber;
        [SerializeField, Expandable] LerpConfigSO lerpConfigSO;
        [SerializeField] Color specialImageColor;
        [SerializeField] Color specialImageFilledColor;

        Tween healthLerpTween;
        Coroutine healthCoroutine;
        PlayerCharacter Player => GameManager.Instance.PlayerCharacterList[playerNumber - 1];
        private void Awake()
        {
            InstantHealthBar.fillAmount = 1f;
            LerpHealthBar.fillAmount = 1f;
        }
        private void Update()
        {
            if (GameManager.Instance.PlayerCharacterList.Count < playerNumber)
            {
                if(gameObject.activeInHierarchy)
                gameObject.SetActive(false);

                return;
            }
            scrapCountText.text = Player.scrapAmount.ToString("D3");
            playerIcon.sprite = Player.Icon;
            playerIcon.SetNativeSize();
        }

        public void UpdateStoredItem(int playerIndex)
        {
            if (playerIndex + 1 != playerNumber) return;

            Sprite newIcon = GameManager.Instance.PlayerCharacterList[playerNumber - 1].ItemIcon;

            itemIcon.sprite = newIcon;

            if (newIcon != null) itemIcon.enabled = true;
            else itemIcon.enabled = false;


            itemIcon.SetNativeSize();
        }

        public void UpdateHealthBar(IntFloat playerHealthParams)
        {
            if (playerHealthParams.Int != playerNumber - 1) return;

            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
            }
            healthCoroutine = StartCoroutine(LerpHealthRoutine(playerHealthParams.Float));
        }
        public IEnumerator LerpHealthRoutine(float newHealthPercent)
        {
            if (healthLerpTween != null) healthLerpTween.Kill();
            InstantHealthBar.fillAmount = newHealthPercent;
            yield return new WaitForSeconds(lerpConfigSO.Delay);
            healthLerpTween = LerpHealthBar.DOFillAmount(newHealthPercent,
                lerpConfigSO.Duration).SetEase(lerpConfigSO.Ease);
            healthCoroutine = null;
        }

        public void UpdateSpecialBar(IntFloat intFloat)
        {
            if (intFloat.Int != playerNumber - 1) return;

            float newPercent = intFloat.Float;
            int charges = specialImages.Length;
            float perChargePercent = 1f / charges;

            for (int i = 0; i < charges; i++)
            {
                float thisMin = i * perChargePercent;
                float thisMax = (i + 1f) * perChargePercent;

                if(newPercent >= thisMax)
                {
                    specialImages[i].fillAmount = 1f;
                    specialImages[i].color = specialImageFilledColor;
                }
                else if(newPercent < thisMin)
                {
                    specialImages[i].fillAmount = 0f;
                    specialImages[i].color = specialImageColor;
                }
                else
                {
                    specialImages[i].fillAmount = (newPercent - thisMin).Map(0, perChargePercent, 0, 1);
                    specialImages[i].color = specialImageColor;
                }
            }
        }
    }
}
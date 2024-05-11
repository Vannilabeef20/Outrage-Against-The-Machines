using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

namespace Game
{
    public class PlayerHealthHandler : MonoBehaviour, IDamageble
    {
        [SerializeField, TextArea] private string Comment;
        [Header("REFERENCES"), HorizontalLine]
        [SerializeField] private PlayerStateMachine playerStateMachine;
        [SerializeField] private SpriteRenderer spriteRenderer;
        public CapsuleCollider playerHitbox;

        #region Player Health Params
        [Header("HEALTH PARAMS"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] private ParticleSystem healParticle;

        [SerializeField] private LayerMask hostileLayers;

        [SerializeField] private float maxHeathPoints;
        [field: SerializeField, ProgressBar("HP", "maxHeathPoints", EColor.Red)] public float CurrentHealthPoints { get; private set; }

        [SerializeField, ReadOnly] private bool isDead;

        #endregion
        #region Stagger Params
        [Header("STAGGER PARAMS"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField] private float hitFlashLenght;

        [SerializeField] private Color hitFlashColor;

        [Space]

        [SerializeField] private float staggerGracePeriod;

        [SerializeField] private float gracePeriodFlashLenght;

        [SerializeField] private Color gracePeriodColor;
        #endregion
        #region UI
        [Header("UI"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField] private PlayerHitParamsEvent OnHealthChanged;
        #endregion



        private void Awake()
        {
            spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
            CurrentHealthPoints = maxHeathPoints;
        }

        public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            isDead = false;
            playerHitbox.enabled = false;
            float formerHealthPercent = CurrentHealthPoints / maxHeathPoints;
            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0f, maxHeathPoints);
            float newHealthPercent = CurrentHealthPoints / maxHeathPoints;
            OnHealthChanged.Raise(this, new PlayerHitParams(playerStateMachine.playerInput.playerIndex, 1f, CurrentHealthPoints / maxHeathPoints));

            if (CurrentHealthPoints <= 0f)
            {
                isDead = true;
            }
            else
            {
                isDead = false;
            }

            if (damageDealerPos.x - transform.position.x >= 0)
            {
                playerStateMachine.TakeDamage(isDead, -knockbackStrenght * Vector2.right, stunDuration);
            }
            else
            {
                playerStateMachine.TakeDamage(isDead, knockbackStrenght * Vector2.right, stunDuration);
            }

            if(isDead == false)
            {
                StartCoroutine(StunRoutine(stunDuration));
            }
            else
            {
                StartCoroutine(StunRoutine(playerStateMachine.Death.Duration + playerStateMachine.Death.Delay));
            }
        }

        public IEnumerator StunRoutine(float stunDuration)
        {
            float UpTime = 0f;
            float flashTime = 0f;
            Color startColor = spriteRenderer.color;
  
            while(UpTime < stunDuration)
            {
                playerHitbox.enabled = false;
                UpTime += Time.deltaTime;
                flashTime += Time.deltaTime;
                if (flashTime > hitFlashLenght)
                {
                    if (spriteRenderer.color == hitFlashColor)
                    {
                        spriteRenderer.color = startColor;
                    }
                    else
                    {
                        spriteRenderer.color = hitFlashColor;
                    }
                    flashTime = 0;
                }
                yield return null;
            }
            if (isDead)
            {
                Revive();
            }
            while (UpTime < stunDuration + staggerGracePeriod)
            {
                playerHitbox.enabled = false;
                UpTime += Time.deltaTime;
                flashTime += Time.deltaTime;
                if (flashTime > gracePeriodFlashLenght)
                {
                    if (spriteRenderer.color == gracePeriodColor)
                    {
                        spriteRenderer.color = startColor;
                    }
                    else
                    {
                        spriteRenderer.color = gracePeriodColor;
                    }
                    flashTime = 0;
                }
                yield return null;
            }
            spriteRenderer.color = startColor;
            playerHitbox.enabled = true; 
        }

        public void Heal(float healPercent)
        {
            healParticle.Play();
            float formerHealthPercent = CurrentHealthPoints / maxHeathPoints;
            CurrentHealthPoints += (maxHeathPoints * healPercent);
            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints, 0f, maxHeathPoints);
            float newHealthPercent = CurrentHealthPoints / maxHeathPoints;
            OnHealthChanged.Raise(this, new PlayerHitParams(playerStateMachine.playerInput.playerIndex, formerHealthPercent, newHealthPercent));
        }
        
        public void Revive()
        {
            float formerHealthPercent = CurrentHealthPoints / maxHeathPoints;
            CurrentHealthPoints = maxHeathPoints;
            float newHealthPercent = CurrentHealthPoints / maxHeathPoints;
            OnHealthChanged.Raise(this, new PlayerHitParams(playerStateMachine.playerInput.playerIndex, formerHealthPercent, newHealthPercent));
            playerHitbox.enabled = true;
        }
    }
}

using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using FMODUnity;

namespace Game
{
    public class PlayerHealthHandler : MonoBehaviour, IDamageble
    {
        [SerializeField, TextArea] private string Comment;
        [Header("REFERENCES"), HorizontalLine]
        [SerializeField] PlayerStateMachine playerStateMachine;
        [SerializeField] SpriteRenderer spriteRenderer;
        public CapsuleCollider playerHitbox;

        [SerializeField] IntFloatEvent healthEvent;
        [SerializeField] ParticleSystem healParticle;
        [SerializeField] ParticleSystem damageParticle;
        [SerializeField] StudioEventEmitter reviveEmitter;

        #region Player Health Params
        [Header("HEALTH PARAMS"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] LayerMask hostileLayers;
        [SerializeField] float maxHeathPoints;
        [field: SerializeField, ProgressBar("HP", "maxHeathPoints", EColor.Red)] public float CurrentHealthPoints { get; private set; }
        [SerializeField, ReadOnly] bool isDead;

        #endregion
        #region Stagger Params
        [Header("STAGGER PARAMS"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField] float hitFlashLenght;
        [SerializeField] Color hitFlashColor;

        [Space]

        [SerializeField] float staggerGracePeriod;
        [SerializeField] float gracePeriodFlashLenght;
        [SerializeField] Color gracePeriodColor;

        [Header("REVIVE PARAMS"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField, Min(0)] float reviveRumbleDuration = 0.5f;
        [SerializeField, Range(0f, 1f)] float reviveRumble_lowFreq = 0.5f;
        [SerializeField, Range(0f, 1f)] float reviveRumble_highFreq = 0.5f;
        #endregion

        int PlayerIndex => playerStateMachine.playerInput.playerIndex;

        private void Awake()
        {
            spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
            CurrentHealthPoints = maxHeathPoints;
        }

        public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            isDead = false;
            playerHitbox.enabled = false;
            damageParticle.Play();
            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0f, maxHeathPoints);
            float newHealthPercent = CurrentHealthPoints / maxHeathPoints;
            healthEvent.Raise(this, new IntFloat(PlayerIndex, newHealthPercent));

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

        public void Heal(float healPercent, float healFlat = 0f)
        {
            healParticle.Play();
            CurrentHealthPoints += (maxHeathPoints * healPercent/100) + healFlat;
            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints, 0f, maxHeathPoints);
            float newHealthPercent = CurrentHealthPoints / maxHeathPoints;
            healthEvent.Raise(this, new IntFloat(PlayerIndex, newHealthPercent));
        }

        public void Revive()
        {
            CurrentHealthPoints = maxHeathPoints;
            float newHealthPercent = CurrentHealthPoints / maxHeathPoints;
            healthEvent.Raise(this, new IntFloat(PlayerIndex, newHealthPercent));
            playerHitbox.enabled = true;
            reviveEmitter.Play();
            foreach (var device in playerStateMachine.playerInput.devices)
            {
                GameManager.Instance.Rumble(device, reviveRumble_lowFreq, reviveRumble_highFreq, reviveRumbleDuration);
            }
        }
        
    }
}

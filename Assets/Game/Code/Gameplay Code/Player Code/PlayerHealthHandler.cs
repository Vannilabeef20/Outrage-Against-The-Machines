using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using DG.Tweening;
using FMODUnity;

namespace Game
{
    public class PlayerHealthHandler : MonoBehaviour, IDamageble
    {
        [Header("REFERENCES"), HorizontalLine]
        [SerializeField] PlayerInput playerInput;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] CapsuleCollider playerHitbox;

        [SerializeField] IntFloatEvent healthEvent;
        [SerializeField] ParticleSystem healParticle;
        [SerializeField] ParticleSystem damageParticle;
        [SerializeField] StudioEventEmitter reviveEmitter;

        #region Player Health Params
        [Header("HEALTH PARAMS"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] float maxHeathPoints;
        [field: SerializeField, ProgressBar("HP", "maxHeathPoints", EColor.Red)] public float CurrentHealthPoints { get; private set; }

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

        [SerializeField] RumbleData reviveRumble;
        #endregion

        /// <summary>
        /// Vector2 = KnockbackForce, float = Stun Duration
        /// </summary>
        public event Action<Vector2, float> OnDamageTaken;
        /// <summary>
        /// Vector2 = KnockbackForce, float = Stun Duration
        /// </summary>
        public event Action<Vector2, float> OnDeath;

        public event Action OnRevive;

        int PlayerIndex => playerInput.playerIndex;
        string RumbleId => $"P{PlayerIndex + 1} Revive";

        private void Awake()
        {
            spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
            CurrentHealthPoints = maxHeathPoints;
        }

        public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            playerHitbox.enabled = false;
            damageParticle.Play();

            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0f, maxHeathPoints);
            float newHealthPercent = CurrentHealthPoints / maxHeathPoints;
            healthEvent.Raise(this, new IntFloat(PlayerIndex, newHealthPercent));

            bool dead = CurrentHealthPoints <= 0;

            Vector3 knockbackDir;
            if (damageDealerPos.x - transform.position.x >= 0) 
                knockbackDir = Vector2.right;
            else 
                knockbackDir = Vector2.left;

            if (dead) OnDeath.Invoke(knockbackStrenght * knockbackDir, stunDuration);
            else OnDamageTaken.Invoke(knockbackStrenght * knockbackDir, stunDuration);
        }
        
        public void StartGracePeriod()
        {
            StartCoroutine(GracePeriodRoutine());
        }

        public void Stun(float duration)
        {
            StartCoroutine(StunRoutine(duration));
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
            RumbleManager.Instance.CreateRumble(RumbleId, reviveRumble, PlayerIndex);
            OnRevive.Invoke();
            StartCoroutine(GracePeriodRoutine());;
        }
        IEnumerator StunRoutine(float stunDuration)
        {
            float UpTime = 0f;
            float flashTime = 0f;
            Color startColor = spriteRenderer.color;

            while (UpTime < stunDuration)
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
            spriteRenderer.color = startColor;
            StartGracePeriod();
        }

        IEnumerator GracePeriodRoutine()
        {
            float UpTime = 0f;
            float flashTime = 0f;
            Color startColor = spriteRenderer.color;
            while (UpTime < staggerGracePeriod)
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
    }
}

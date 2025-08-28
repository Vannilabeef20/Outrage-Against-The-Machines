using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
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
        [SerializeField] BoxCollider enemyCollision;

        [SerializeField] IntFloatEvent healthEvent;
        [SerializeField] ParticleSystem healParticle;
        [SerializeField] ParticleSystem damageParticle;
        [SerializeField] StudioEventEmitter reviveEmitter;
        [SerializeField] CinemachineImpulseSource impulseSource;

        #region Player Health Params
        [Header("HEALTH PARAMS"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] float maxHeathPoints;
        [field: SerializeField, ProgressBar("HP", "maxHeathPoints", EColor.Red)] public float CurrentHealthPoints { get; private set; }
        [ReadOnly] public float damageMultiplier = 1f;

        #endregion
        #region Stagger Params
        [Header("STAGGER PARAMS"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField] RumbleData hitRumble;
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
        /// Vector2 = KnockbackForce, float = PlayHitEffect Duration
        /// </summary>
        public event Action<float, Vector3, float> OnDamageTaken;
        /// <summary>
        /// Vector2 = KnockbackForce, float = PlayHitEffect Duration
        /// </summary>
        public event Action<Vector2, float> OnDeath;

        public event Action OnRevive;

        int PlayerIndex => playerInput.playerIndex;
        string RumbleId => $"P{playerInput.playerIndex + 1} {gameObject.name}";


        [Button]
        public void UpdateHealthUI()
        {
            healthEvent.Raise(this, new IntFloat(PlayerIndex, CurrentHealthPoints / maxHeathPoints));
        }

        [Button]
        void Kill()
        {
            TakeDamage(transform.position, maxHeathPoints, 0.5f, 0);
        }

        private void Awake()
        {
            spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
            CurrentHealthPoints = maxHeathPoints;
        }

        public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
        {
            damageParticle.Play();
            damage *= damageMultiplier;

            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0f, maxHeathPoints);
            UpdateHealthUI();

            Vector3 knockbackDir = transform.position - damageDealerPos;
            knockbackDir.y = 0;
            knockbackDir.z = 0;
            knockbackDir.Normalize();
           

            //if dead
            if (CurrentHealthPoints <= 0)
            {
                playerHitbox.enabled = false;
                enemyCollision.enabled = false;
                OnDeath.Invoke(knockbackStrenght * knockbackDir, stunDuration);
                return;
            }
            
            //If alive
            StartCoroutine(StunRoutine(stunDuration));
            RumbleManager.Instance.CreateRumble(RumbleId + " Damage", hitRumble, playerInput.playerIndex);
            impulseSource.GenerateImpulse();
            OnDamageTaken.Invoke(damage, knockbackStrenght * knockbackDir, stunDuration);           
        }
        
        public void StartGracePeriod()
        {
            StartCoroutine(GracePeriodRoutine());
        }


        public void Heal(float healPercent, float healFlat = 0f)
        {
            healParticle.Play();
            float newHealth = CurrentHealthPoints;
            newHealth += (maxHeathPoints * healPercent/100) + healFlat;
            newHealth = Mathf.Clamp(newHealth, 0f, maxHeathPoints);
            CurrentHealthPoints = newHealth;
            UpdateHealthUI();

            UpdateHealthUI();
        }

        public void Revive()
        {
            CurrentHealthPoints = maxHeathPoints;
            UpdateHealthUI();
            playerHitbox.enabled = true;
            enemyCollision.enabled = true;
            reviveEmitter.Play();
            RumbleManager.Instance.CreateRumble(RumbleId + " Revive", reviveRumble, PlayerIndex);
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
                enemyCollision.enabled = false;
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
            playerHitbox.enabled = false;
            enemyCollision.enabled = false;
            while (UpTime < staggerGracePeriod)
            {
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
            enemyCollision.enabled = true;
        }
    }
}

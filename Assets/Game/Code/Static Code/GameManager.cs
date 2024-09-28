using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using FMODUnity;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance{ get; private set; }
        [field: SerializeField, ReadOnly] public Camera MainCamera { private set; get; }

        [SerializeField] GameObject root;

        [SerializeField] MenuIdEvent OnSetMenuVisibility;

        #region Players Params
        [field: Header("PLAYERS"), HorizontalLine(2f, EColor.Orange)]

        public PlayerInputManager UnityInputManager;

        [field: Space]
        [field: SerializeField, Tag] public string[] PlayerTags { get; private set; }
        [field: SerializeField] public PlayerCharacter DefaultPlayer { get; private set; }

        [field: SerializeField, ReadOnly] public List<PlayerCharacter> PlayerCharacterList { get; private set; }

        [SerializeField] GameObject followGroupPrefab;
        #endregion

        #region Lifes Params
        [Header("REVIVE & LIVES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, Min(0), MaxValue(1f)] Vector2 spawnViewportPostion;
        [SerializeField] IntEvent UpdateLifeCount;
        [field: SerializeField, ReadOnly] public int CurrentLifeAmount { get; private set; }

        [SerializeField] int initialLifeAmout;
        [SerializeField] int maxLifeAmount;

        #endregion

        [Header("RUMBLE"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] bool isRumbleEnabled;

        #region Debug
        [Header("DEBUG"), HorizontalLine(2f, EColor.Blue)]
        [SerializeField] GUIStyle RespawnLabelStyle;
        #endregion

        #region Unity/Application Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
#if UNITY_EDITOR
            EditorApplication.quitting += StopRumble;
#endif
        }
        private void Start()
        {
            MainCamera = Camera.main;
            int level = SceneManager.GetActiveScene().buildIndex;
            if (level == 0)
            {
                OnSetMenuVisibility.Raise(this, EMenuId.StartMenu);
            }
            else
            {
                InitializeLevel();
                CurrentLifeAmount = initialLifeAmout;
                UpdateLifeCount.Raise(this, CurrentLifeAmount);
                OnSetMenuVisibility.Raise(this, EMenuId.None);               
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            if (MainCamera == null) MainCamera = Camera.main;

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if(Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (level == 0) OnSetMenuVisibility.Raise(this, EMenuId.StartMenu);
            else
            {
                OnSetMenuVisibility.Raise(this, EMenuId.None);
                InitializeLevel();
                UnityInputManager.playerPrefab = root;
            }           
        }

        private void OnApplicationQuit()
        {
            StopRumble();
        }
        #endregion

        private void InitializeLevel()
        {
            MainCamera = FindObjectOfType<Camera>();
            CurrentLifeAmount = initialLifeAmout;
            UpdateLifeCount.Raise(this, CurrentLifeAmount);

            if (PlayerCharacterList.Count == 0) //Starting the game at a gameplay scene
            {
                PlayerCharacterList.Add(DefaultPlayer);
                PlayerCharacterList[0].GameObject = Instantiate(PlayerCharacterList[0].Prefab, LevelManager.Instance.SpawnCoordinates[0], Quaternion.identity);
                PlayerCharacterList[0].Transform = PlayerCharacterList[0].GameObject.transform;
                PlayerCharacterList[0].Transform.position = LevelManager.Instance.SpawnCoordinates[0];
                PlayerCharacterList[0].HealthHandler = PlayerCharacterList[0].GameObject.GetComponentInChildren<PlayerHealthHandler>();
                PlayerCharacterList[0].isPlayerActive = true;
            }
            else //Starting the game via main menu like normal
            {
                for (int i = 0; i < PlayerCharacterList.Count; i++) 
                {
                    UnityInputManager.playerPrefab = PlayerCharacterList[i].Prefab;

                    PlayerCharacterList[i].GameObject = UnityInputManager.JoinPlayer(i, -1,
                        PlayerCharacterList[i].ControlScheme, PlayerCharacterList[i].Devices).gameObject;
                    PlayerCharacterList[i].Transform = PlayerCharacterList[i].GameObject.transform;
                    Rigidbody rb = PlayerCharacterList[i].GameObject.GetComponent<Rigidbody>();
                    rb.position = LevelManager.Instance.SpawnCoordinates[PlayerCharacterList[i].Index];
                    PlayerCharacterList[i].HealthHandler = PlayerCharacterList[i].GameObject.GetComponentInChildren<PlayerHealthHandler>();
                    PlayerCharacterList[i].isPlayerActive = true;
                }
            }
            Instantiate(followGroupPrefab);
        }

        #region Rumble Methods
        public void Rumble(InputDevice device, float lowFrequency, float highFrequency, float duration)
        {
            if (!isRumbleEnabled) return;

            Gamepad gamepad;
            try
            {
                gamepad = (Gamepad)device;
            }
            catch
            {
                return;
            }

            StartCoroutine(PulseRumble(gamepad, lowFrequency, highFrequency, duration));
        }
        public void Rumble(float lowFrequency, float highFrequency, float duration)
        {
            if (!isRumbleEnabled) return;
            foreach (Gamepad gamepad in Gamepad.all)
            {
                StartCoroutine(PulseRumble(gamepad, lowFrequency, highFrequency, duration));
            }
        }

        private IEnumerator PulseRumble(Gamepad gamepad, float lowFrequency, float highFrequency, float duration)
        {
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            yield return new WaitForSecondsRealtime(duration);
            StopRumble(gamepad);
        }
        private IEnumerator PulseRumble(Gamepad[] gamepads, float lowFrequency, float highFrequency, float duration)
        {
            foreach(Gamepad gamepad in gamepads)
            {
                gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            }
            yield return new WaitForSecondsRealtime(duration);
            StopRumble();
        }
        private void StopRumble(Gamepad gamepad)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
        private void StopRumble()
        {
            foreach(var gamepad in Gamepad.all)
            {
                gamepad.SetMotorSpeeds(0f, 0f);
            }
        }

        public void ToggleRumble(bool enabled)
        {
            if (enabled)
            {
                PlayerPrefs.GetInt("Rumble", 1);
            }
            else
            {
                PlayerPrefs.GetInt("Rumble", 0);
            }
            isRumbleEnabled = enabled;
        }
        #endregion

        public void PauseGame()
        {
            if (TransitionManager.Instance.IsTransitioning) return;

            if (Time.timeScale > 0)
            {
                OnSetMenuVisibility.Raise(this, EMenuId.PauseMenu);
            }
            else
            {
                OnSetMenuVisibility.Raise(this, EMenuId.None);
            }
        }
        
        
        public void TakeAddLife(int amount)
        {
            CurrentLifeAmount = Mathf.Clamp(CurrentLifeAmount + amount, 0, maxLifeAmount);
            for(int i = 0; i < PlayerCharacterList.Count; i++)
            {
                if (!PlayerCharacterList[i].isPlayerActive && CurrentLifeAmount > 0)
                {
                    PlayerCharacterList[i].Transform.position = 
                        MainCamera.ViewportToWorldPoint(spawnViewportPostion).ToXYY();

                    CurrentLifeAmount = Mathf.Clamp(CurrentLifeAmount--, 0, maxLifeAmount);
                    PlayerCharacterList[i].GameObject.SetActive(true);
                    PlayerCharacterList[i].HealthHandler.playerHitbox.enabled = true;
                    PlayerCharacterList[i].isPlayerActive = true;
                }
            }
            UpdateLifeCount.Raise(this, CurrentLifeAmount);
        }
        
        public void UpdatePlayerDeathStatus(PlayerDeathParams playerDeathParams)
        {
            PlayerCharacterList[playerDeathParams.playerIndex].isPlayerActive = !playerDeathParams.isPlayerDead;
            int aliveCount = 0;
            foreach(var character in PlayerCharacterList)
            {
                if(character.isPlayerActive)
                {
                    aliveCount++;
                }
            }
            if(aliveCount == 0)
            {
                TransitionManager.Instance.LoadScene(0);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (MainCamera == null) return;
            Gizmos.color = RespawnLabelStyle.normal.textColor;
            Vector3 respawnPos = MainCamera.ViewportToWorldPoint(spawnViewportPostion).ToXYY();
            Vector3 centerPos = MainCamera.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            Vector3 xPos = MainCamera.ViewportToWorldPoint(new Vector3(spawnViewportPostion.x, 0f, 0f).ToXYY());
            Vector3 yPos = MainCamera.ViewportToWorldPoint(new Vector3(0f, spawnViewportPostion.y, 0f).ToXYY());

            Handles.Label(respawnPos, $"Respawn point", RespawnLabelStyle);
            Gizmos.DrawSphere(respawnPos, 0.1f);

            Handles.DrawDottedLine(new Vector3(respawnPos.x, respawnPos.y, centerPos.z), xPos, 0.2f);
            xPos.y -= 0.3F;
            Handles.Label(xPos,"X");

            Handles.DrawDottedLine(new Vector3(respawnPos.x, respawnPos.y, centerPos.z), yPos, 0.2f);
            yPos.x -= 0.3f;
            Handles.Label(yPos, "Y");
        }
#endif
    }

    [Serializable]
    public class PlayerCharacter
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }
        [field: Space]
        [field: SerializeField, ReadOnly, AllowNesting] public int Index { get; private set; }
        [field: SerializeField, ReadOnly, AllowNesting] public string ControlScheme { get; private set; }
        [field: SerializeField, ReadOnly, AllowNesting] public InputDevice[] Devices { get; private set; }

        [field: SerializeField, ReadOnly, AllowNesting] public GameObject StoredItem { get; private set; }
        [field: SerializeField, ReadOnly, ShowAssetPreview, AllowNesting] public Sprite ItemIcon { get; private set; }
        [Space]
        [ReadOnly, AllowNesting] public bool isPlayerActive;
        [ReadOnly, AllowNesting] public int scrapAmount;

        [Space]
        [ReadOnly, AllowNesting] public GameObject GameObject;
        [ReadOnly, AllowNesting] public Transform Transform;
        [ReadOnly, AllowNesting] public PlayerHealthHandler HealthHandler;

        public bool HasItemStored => StoredItem != null;

        public PlayerCharacter (GameObject playerPrefab, int playerIndex, Sprite playerIcon, string controlScheme, InputDevice[] devices)
        {
            Prefab = playerPrefab;
            Index = playerIndex;
            Icon = playerIcon;
            ControlScheme = controlScheme;
            Devices = devices;
            StoredItem = null;
            ItemIcon = null;
        }

        public void StoreItem(GameObject item, Sprite icon)
        {
            item.SetActive(false);
            StoredItem = item;
            ItemIcon = icon;
        }

        public void RemoveItem()
        {
            StoredItem = null;
            ItemIcon = null;
        }
    }
}


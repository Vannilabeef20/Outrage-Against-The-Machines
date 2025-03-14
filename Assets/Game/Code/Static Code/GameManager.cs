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
        [field: SerializeField] public Vector3[] SpawnCoordinates { private set; get; }
        #endregion

        #region Lifes Params
        [Header("REVIVE & LIVES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, Min(0), MaxValue(1f)] Vector2 spawnViewportPostion;
        [SerializeField] IntEvent UpdateLifeCount;
        [field: SerializeField, ReadOnly] public int CurrentLifeAmount { get; private set; }

        [SerializeField] int initialLifeAmout;
        [SerializeField] int maxLifeAmount;

        #endregion

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
                int level = SceneManager.GetActiveScene().buildIndex;
                if(level == 1)
                InitializeLevel();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        private void Start()
        {
            MainCamera = Camera.main;
            int level = SceneManager.GetActiveScene().buildIndex;
            if (level != 1)
            {
                OnSetMenuVisibility.Raise(this, EMenuId.StartMenu);
            }
            else
            {
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

            if (level != 1)
            {
                OnSetMenuVisibility.Raise(this, EMenuId.StartMenu);
            }
            else
            {
                foreach (PlayerCharacter player in PlayerCharacterList)
                {
                    player.scrapAmount = 0;
                }
                OnSetMenuVisibility.Raise(this, EMenuId.None);
                InitializeLevel();
                UnityInputManager.playerPrefab = root;
            }
        }
        #endregion

        private void InitializeLevel()
        {
            MainCamera = FindAnyObjectByType<Camera>();
            CurrentLifeAmount = initialLifeAmout;
            UpdateLifeCount.Raise(this, CurrentLifeAmount);

            PlayerCharacter player;

            if (PlayerCharacterList.Count == 0) //Starting the game at a gameplay scene
            {
                player = DefaultPlayer;

                PlayerCharacterList.Add(player);
                player.GameObject = Instantiate(player.Prefab, SpawnCoordinates[0], Quaternion.identity);
                player.Transform = player.GameObject.transform;
                player.Transform.position = SpawnCoordinates[0];
                player.InitialDevices = InputSystem.devices.ToArray();
                player.Input = player.GameObject.GetComponent<PlayerInput>();
            }
            else //Starting the game via main menu like normal
            {
                for (int i = 0; i < PlayerCharacterList.Count; i++) 
                {
                    player = PlayerCharacterList[i];

                    UnityInputManager.playerPrefab = player.Prefab;

                    player.GameObject = UnityInputManager.JoinPlayer(i, -1,
                        player.InitialControlScheme, player.InitialDevices).gameObject;
                    player.Transform = player.GameObject.transform;
                    Rigidbody rb = player.GameObject.GetComponent<Rigidbody>();
                    rb.position = SpawnCoordinates[player.Index];
                    player.Input = player.GameObject.GetComponent<PlayerInput>();
                }
            }
        }

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
            float Current = CurrentLifeAmount;
            CurrentLifeAmount = Mathf.Clamp(CurrentLifeAmount + amount, 0, maxLifeAmount);
            float PostTakeAdd = CurrentLifeAmount;
            for(int i = 0; i < PlayerCharacterList.Count; i++)
            {
                if (CurrentLifeAmount == 0) break;

                if (!PlayerCharacterList[i].GameObject.activeInHierarchy && CurrentLifeAmount > 0)
                {
                    PlayerCharacterList[i].Transform.position = 
                        MainCamera.ViewportToWorldPoint(spawnViewportPostion).ToXYY();

                    PlayerCharacterList[i].GameObject.GetComponentInChildren<PlayerHealthHandler>().Revive();
                    CurrentLifeAmount = Mathf.Clamp(CurrentLifeAmount - 1, 0, maxLifeAmount);
                    float PostRevive = CurrentLifeAmount;
                    Debug.Log($"{amount} {Current} {PostTakeAdd} {PostRevive}");
                }
            }
            UpdateLifeCount.Raise(this, CurrentLifeAmount);
        }
        
        public bool OnScreenPercent(float percent, params Vector3[] points)
        {
            percent = Mathf.Clamp01(percent);
            int count = 0;
            Vector2 viewPortPoint;
            foreach (var worldPoint in points)
            {
                viewPortPoint = MainCamera.WorldToViewportPoint(worldPoint);
                if (viewPortPoint.InsideRange(Vector2.zero, Vector2.one))
                {
                    count++;
                }
            }
            return count >= points.Length * percent;
        }
        public bool OnScreen(params Vector3[] points)
        {
            Vector2 viewPortPoint;
            foreach (var worldPoint in points)
            {
                viewPortPoint = MainCamera.WorldToViewportPoint(worldPoint);

                if (viewPortPoint.InsideRange(Vector2.zero, Vector2.one)) return true;
            }
            return false;
        }


#if UNITY_EDITOR
        [SerializeField] int addMoney;
        [SerializeField] GUIStyle SpawnLabelStyle;
        [Button("ADD MONEY ALL", EButtonEnableMode.Playmode)]
        void AddMoney()
        {
            foreach(PlayerCharacter player in PlayerCharacterList)
            {
                player.scrapAmount += addMoney;
            }
        }

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

            Gizmos.color = Color.yellow;
            for (int i = 0; i < SpawnCoordinates.Length; i++)
            {
                Gizmos.DrawSphere(SpawnCoordinates[i], 0.1f);
                Handles.Label(SpawnCoordinates[i], $"Spawn P{i + 1}", SpawnLabelStyle);
            }
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
        [field: SerializeField, ReadOnly, AllowNesting] public string InitialControlScheme { get; private set; }

        [SerializeField, ReadOnly, AllowNesting] public InputDevice[] InitialDevices;

        [field: SerializeField, ReadOnly, AllowNesting] public GameObject StoredItem { get; private set; }
        [field: SerializeField, ReadOnly, ShowAssetPreview, AllowNesting] public Sprite ItemIcon { get; private set; }
        [Space]
        [ReadOnly, AllowNesting] public bool isDead;
        [ReadOnly, AllowNesting] public int scrapAmount;

        [Space]
        [ReadOnly, AllowNesting] public PlayerInput Input;
        [ReadOnly, AllowNesting] public GameObject GameObject;
        [ReadOnly, AllowNesting] public Transform Transform;

        public bool HasItemStored => StoredItem != null;

        public PlayerCharacter (GameObject playerPrefab, int playerIndex, Sprite playerIcon, string controlScheme, InputDevice[] devices)
        {
            Prefab = playerPrefab;
            Index = playerIndex;
            Icon = playerIcon;
            InitialControlScheme = controlScheme;
            InitialDevices = devices;
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


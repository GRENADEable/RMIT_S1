using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim_F2
{
    public class GameManagerPlatformDuel : MonoBehaviour
    {
        #region Serialized Variables

        #region Datas
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerDataMiniGame gmData = default;

        [SerializeField]
        [Tooltip("PlayerVisual Scriptable Object")]
        private PlayerVisualData[] playerVisData = default;

        [SerializeField]
        [Tooltip("Do you want to disable Cursor?")]
        private bool isCursorDisabled = default;
        #endregion

        #region UI
        [Space, Header("UI")]
        [SerializeField]
        [Tooltip("Fade panel Animation Component")]
        private Animator fadeBG = default;

        [SerializeField]
        [Tooltip("Timer Panel")]
        private GameObject timerPanel = default;

        [SerializeField]
        [Tooltip("Timer Text")]
        private TextMeshProUGUI timerText = default;

        [SerializeField]
        [Tooltip("HUD Panel")]
        private GameObject hudPanel = default;

        [SerializeField]
        [Tooltip("Player UI GameObject Prefab")]
        private GameObject playerScorePrefab = default;

        [SerializeField]
        [Tooltip("Player UI Spawn Pos")]
        private Transform playerScorePos = default;
        #endregion

        #region Game
        [Space, Header("Game")]
        [SerializeField]
        [Tooltip("Player Spawn Pos")]
        private Transform playerSpawnPos = default;
        #endregion

        #region Events Bool
        public delegate void SendEventsBool(bool isSwitched);
        /// <summary>
        /// Event sent from GameManagerPlatformDuel to PlayerControllerBall Script;
        /// Inverts the Player Controls;
        /// </summary>
        public static event SendEventsBool OnControlsSwitched;

        /// <summary>
        /// Event sent from GameManagerPlatformDuel to PlayerControllerBall Script;
        /// Stops the player from Jumping;
        /// </summary>
        public static event SendEventsBool OnControlsJump;
        #endregion

        #region Events Void
        public delegate void SendEvents(Color clr, string playerName);
        #endregion

        #endregion

        #region Private Variables
        private bool _isSwitchingControls;
        [Header("Game")]
        private List<GameObject> _players = new List<GameObject>();
        private int _currPlayerNo = default;
        [SerializeField] private List<GameObject> _playersUI = new List<GameObject>();
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            PlayerControllerBall.OnPlayerIntialised += OnPlayerIntialisedEventReceived;
        }

        void OnDisable()
        {
            PlayerControllerBall.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
        }

        void OnDestroy()
        {
            PlayerControllerBall.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
        }
        #endregion

        void Start()
        {
            StartCoroutine(IntroDelay());

            if (isCursorDisabled)
                gmData.DisableCursor();
        }

        void Update()
        {
            // Testing Control Switch;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _isSwitchingControls = !_isSwitchingControls;
                OnControlsSwitched?.Invoke(_isSwitchingControls);
                OnControlsJump?.Invoke(!_isSwitchingControls);
            }
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Sets the UI of the player depending on how many players joined;
        /// </summary>
        void SetPlayersUI()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                GameObject plyUI = Instantiate(playerScorePrefab, playerScorePos.position, Quaternion.identity, playerScorePos);
                _playersUI.Add(plyUI);
            }
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Starts game with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator IntroDelay()
        {
            fadeBG.Play("Fade_In");
            yield return new WaitForSeconds(0.5f);
            gmData.ChangeGameState("Intro");
        }

        /// <summary>
        /// Starts match with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator StartMatchDelay()
        {
            timerPanel.SetActive(true);
            timerText.text = "3";
            yield return new WaitForSeconds(1f);
            timerText.text = "2";
            yield return new WaitForSeconds(1f);
            timerText.text = "1";
            yield return new WaitForSeconds(1f);
            SetPlayersUI();
            timerPanel.SetActive(false);
            hudPanel.SetActive(true);
            Debug.Log("Start");
        }
        #endregion

        #region Events
        public void OnPlayerIntialisedEventReceived(GameObject player)
        {
            _players.Add(player);
            _players[_currPlayerNo].name = $"{playerVisData[_currPlayerNo].playerName}";
            _players[_currPlayerNo].GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", playerVisData[_currPlayerNo].playerColour);
            _players[_currPlayerNo].transform.position = playerSpawnPos.position;
            _currPlayerNo++;

            if (_currPlayerNo == 2)
            {
                gmData.ChangeGameState("Starting");
                StartCoroutine(StartMatchDelay());
            }
        }
        #endregion
    }
}
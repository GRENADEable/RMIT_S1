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
        [Tooltip("Starting Round Timer Text")]
        private TextMeshProUGUI startingRoundTimerText = default;

        [SerializeField]
        [Tooltip("Starting Round Timer Text")]
        private TextMeshProUGUI gameRoundTimerText = default;

        [SerializeField]
        [Tooltip("HUD Panel")]
        private GameObject hudPanel = default;

        [SerializeField]
        [Tooltip("Player UI GameObject Prefab")]
        private GameObject playerScorePrefab = default;

        [SerializeField]
        [Tooltip("Player UI Spawn Pos")]
        private Transform playerScorePos = default;

        [SerializeField]
        [Tooltip("Player Score Increment")]
        private int playerScoreIncrement = 1;
        #endregion

        #region Game
        [Space, Header("Game")]
        [SerializeField]
        [Tooltip("Player Spawn Pos")]
        private Transform playerSpawnPos = default;

        [SerializeField]
        [Tooltip("Starting Round Time")]
        private float startingGameRoundTimer = default;

        [SerializeField]
        [Tooltip("Platform Anim Controller")]
        private Animator platformWallsAnim = default;

        [SerializeField]
        [Tooltip("Player Death Box Col")]
        private GameObject playerDeathBox = default;
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

        #endregion

        #region Private Variables
        private bool _isSwitchingControls;

        [Header("Game")]
        private List<PlayerControllerBall> _playersBall = new List<PlayerControllerBall>();
        private List<PlayerScore> _playersScore = new List<PlayerScore>();
        public int PlayerNo { get => _currPlayerNo; set => _currPlayerNo = value; }
        [SerializeField] private int _currPlayerNo = default;
        [SerializeField] private int _totalPlayerNo = default;
        private float _currGameRoundTime = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            PlayerControllerBall.OnPlayerIntialised += OnPlayerIntialisedEventReceived;
            PlayerControllerBall.OnPlayerFall += OnPlayerFallEventReceived;
        }

        void OnDisable()
        {
            PlayerControllerBall.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
            PlayerControllerBall.OnPlayerFall -= OnPlayerFallEventReceived;
        }

        void OnDestroy()
        {
            PlayerControllerBall.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
            PlayerControllerBall.OnPlayerFall -= OnPlayerFallEventReceived;
        }
        #endregion

        void Start()
        {
            _currGameRoundTime = startingGameRoundTimer;
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

            if (Input.GetKeyDown(KeyCode.T))
                _currGameRoundTime = 2;

            if (gmData.currState == GameManagerDataMiniGame.GameState.Game)
                RoundTimer();
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Sets the UI of the player depending on how many players joined;
        /// </summary>
        void SetPlayersUI()
        {
            int index = 0;

            for (int i = 0; i < _playersBall.Count; i++)
            {
                GameObject plyScore = Instantiate(playerScorePrefab, playerScorePos.position, Quaternion.identity, playerScorePos);
                plyScore.name = playerVisData[i].playerName;

                _playersScore.Add(plyScore.GetComponent<PlayerScore>());
                _playersScore[i].PlayerName = playerVisData[i].playerName;
                _playersScore[i].PlayerPointIndex = index;

                index++;
            }

            gameRoundTimerText.text = startingGameRoundTimer.ToString();
            _currGameRoundTime = startingGameRoundTimer;
            platformWallsAnim.Play("Platform_Duel_Wall_Open");
            gmData.ChangeGameState("Game");
        }

        /// <summary>
        /// Round Timer and also updates the UI for it;
        /// </summary>
        void RoundTimer()
        {
            _currGameRoundTime -= Time.deltaTime;
            gameRoundTimerText.text = _currGameRoundTime.ToString("f1");

            if (_currGameRoundTime <= 0)
            {
                gmData.ChangeGameState("Starting");
                EndRoundWithoutPoint();
            }

        }
        /// <summary>
        /// If one player stands, they win a point;
        /// </summary>
        void EndRoundWithPoint()
        {
            CloseWalls();

            for (int i = 0; i < _playersBall.Count; i++)
            {
                if (_playersBall[i].gameObject.activeInHierarchy)
                {
                    _playersScore[_playersBall[i].PlayerIndex].UpdateScore(playerScoreIncrement);
                    Debug.Log("Updating Score");
                }

                _playersBall[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// If the timer ends, no point;
        /// </summary>
        void EndRoundWithoutPoint()
        {
            CloseWalls();

            for (int i = 0; i < _playersBall.Count; i++)
                _playersBall[i].gameObject.SetActive(false);
        }

        /// <summary>
        /// Resets the state of the game when the round ends;
        /// </summary>
        void CloseWalls()
        {
            platformWallsAnim.Play("Platform_Duel_Wall_Close");
            gmData.ChangeGameState("Starting");
            playerDeathBox.SetActive(false);
            StartCoroutine(EndRoundPointDelay());
        }

        /// <summary>
        /// Continues the game after the reset;
        /// </summary>
        void OpenWalls()
        {
            platformWallsAnim.Play("Platform_Duel_Wall_Open");
            gmData.ChangeGameState("Game");
            playerDeathBox.SetActive(true);
            _currGameRoundTime = startingGameRoundTimer;
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
        /// Starts counter, switches UI Panels and disables more players to join the match after counter is ended;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator StartMatchDelay()
        {
            timerPanel.SetActive(true);
            startingRoundTimerText.text = "3";
            yield return new WaitForSeconds(1f);
            startingRoundTimerText.text = "2";
            yield return new WaitForSeconds(1f);
            startingRoundTimerText.text = "1";
            yield return new WaitForSeconds(1f);
            SetPlayersUI();
            timerPanel.SetActive(false);
            hudPanel.SetActive(true);
            _totalPlayerNo = PlayerNo;
            PlayerInputManager.instance.enabled = false;
        }

        /// <summary>
        /// Ends one round with a Delay;
        /// Checks if all the players haven't falen off the map before giving the point;
        /// </summary>
        /// <returns> Float Delay; </returns>
        //IEnumerator EndRoundPointDelay()
        //{
        //    platformWallsAnim.Play("Platform_Duel_Wall_Close");
        //    gmData.ChangeGameState("Starting");
        //    yield return new WaitForSeconds(1f);
        //    playerDeathBox.SetActive(false);

        //    for (int i = 0; i < _playersBall.Count; i++)
        //    {
        //        if (_playersBall[i].gameObject.activeInHierarchy)
        //        {
        //            _playersScore[_playersBall[i].PlayerIndex].UpdateScore(playerScoreIncrement);
        //            Debug.Log("Updating Score");
        //        }

        //        //if (!_playersBall[i].gameObject.activeInHierarchy)
        //        //{
        //        //    Debug.Log("Draw");
        //        //}

        //        _playersBall[i].gameObject.SetActive(false);
        //    }
        //}

        /// <summary>
        /// Ends one round with a Delay;
        /// Resets the state game back to the initial state;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator EndRoundPointDelay()
        {
            yield return new WaitForSeconds(2f);

            for (int i = 0; i < _playersBall.Count; i++)
            {
                _playersBall[i].transform.position = playerSpawnPos.position;
                _playersBall[i].gameObject.SetActive(true);
            }

            OpenWalls();
            gmData.ChangeGameState("Game");
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to Event from PlayerControllerBall Script;
        /// Adds the GameObject to hte list and starts the match with atleast 2 players joining;
        /// </summary>
        /// <param name="plyBall"> Player GameObject received from Event; </param>
        void OnPlayerIntialisedEventReceived(PlayerControllerBall plyBall)
        {
            _playersBall.Add(plyBall);
            _playersBall[PlayerNo].name = $"{playerVisData[PlayerNo].playerName}";
            _playersBall[PlayerNo].GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", playerVisData[PlayerNo].playerColour);
            _playersBall[PlayerNo].transform.position = playerSpawnPos.position;
            _playersBall[PlayerNo].PlayerIndex = PlayerNo;
            PlayerNo++;

            if (PlayerNo == 2)
            {
                gmData.ChangeGameState("Starting");
                StartCoroutine(StartMatchDelay());
            }
        }

        /// <summary>
        /// Subbed to Event from PlayerControllerBall Script;
        /// Disables the Player GameObject;
        /// </summary>
        /// <param name="index"> Player GameObject affected according to the Index received; </param>
        void OnPlayerFallEventReceived(int index)
        {
            _playersBall[index].gameObject.SetActive(false);
            PlayerNo--;

            if (PlayerNo <= 1)
            {
                //StartCoroutine(EndRoundPointDelay());
                PlayerNo = _totalPlayerNo;
                EndRoundWithPoint();
            }
        }
        #endregion
    }
}
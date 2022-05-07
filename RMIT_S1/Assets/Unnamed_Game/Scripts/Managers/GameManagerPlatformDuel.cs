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

        [SerializeField]
        [Tooltip("End Round Delay")]
        private float endRoundDelay = default;
        #endregion

        #region Platform
        [Space, Header("Platform")]
        [SerializeField]
        [Tooltip("Platform GameObject")]
        private GameObject platform = default;

        [SerializeField]
        [Tooltip("Platform Rotation Speed")]
        [Range(0f, 40f)]
        private float platformRotSpeed = default;

        [SerializeField]
        [Tooltip("Platform Reset Speed when round ends")]
        [Range(0f, 5f)]
        private float platformRoundEndResetSpeed = default;

        [SerializeField]
        [Tooltip("Platform Reset Speed when switching Rotation types")]
        [Range(0f, 5f)]
        private float platformResetSpeed = default;

        [SerializeField]
        [Tooltip("Platform Rotation starting Time")]
        private float startingPlatformRotatingTimer = default;

        [SerializeField]
        [Tooltip("Platform Rotation reset Time")]
        private float resetPlatformTime = default;
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

        #region Game
        [Header("Game")]
        private bool _isSwitchingControls;
        private List<PlayerControllerBall> _playersBall = new List<PlayerControllerBall>();
        private List<PlayerScore> _playersScore = new List<PlayerScore>();
        public int PlayerNo { get => _currPlayerNo; set => _currPlayerNo = value; }
        private int _currPlayerNo = default;
        private int _totalPlayerNo = default;
        private float _currGameRoundTime = default;
        #endregion

        #region Platform
        [Header("Platform")]
        private float _currPlatformRotTime = default;
        private Quaternion _intialPlatformRot = default;
        private enum PlatformRotateType { Original, RotateX, RotateY, RotateZ };
        private PlatformRotateType _currPlatformRotateType = PlatformRotateType.RotateY;
        #endregion

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
            _intialPlatformRot = platform.transform.rotation;
            _currPlatformRotTime = startingPlatformRotatingTimer;

            gmData.ChangeGameState("Intro");
            fadeBG.Play("Fade_In");

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
            {
                RoundTimer();
                RotatePlatform();
            }

            if (gmData.currState == GameManagerDataMiniGame.GameState.Starting)
                ResetRotatePlatform(true);
        }
        #endregion

        #region My Functions

        #region Game
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

            hudPanel.SetActive(true);
            gameRoundTimerText.text = startingGameRoundTimer.ToString();
            _currGameRoundTime = startingGameRoundTimer;
            platformWallsAnim.Play("Platform_Duel_Wall_Open");
            _totalPlayerNo = PlayerNo;
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
                    _playersScore[_playersBall[i].PlayerIndex].UpdateScore(playerScoreIncrement);

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
        #endregion

        #region Platform
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

        /// <summary>
        /// Platform Rotates according to Random Enum chosen;
        /// </summary>
        void RotatePlatform()
        {
            _currPlatformRotTime -= Time.deltaTime;

            if (_currPlatformRotTime <= 0)
            {
                _currPlatformRotTime = startingPlatformRotatingTimer;
                StartCoroutine(ResetRotatePlatformDelay());
            }

            switch (_currPlatformRotateType)
            {
                case PlatformRotateType.RotateX:
                    platform.transform.Rotate(Vector3.right, platformRotSpeed * Time.deltaTime);
                    break;
                case PlatformRotateType.RotateY:
                    platform.transform.Rotate(Vector3.up, platformRotSpeed * Time.deltaTime);
                    break;
                case PlatformRotateType.RotateZ:
                    platform.transform.Rotate(Vector3.forward, platformRotSpeed * Time.deltaTime);
                    break;

                case PlatformRotateType.Original:
                    ResetRotatePlatform(false);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Resets the Rotating Platform when round Ends;
        /// </summary>
        /// <param name="isRoundEnded"> If true, reset it quickly, else reset it slowly; </param>
        void ResetRotatePlatform(bool isRoundEnded)
        {
            if (isRoundEnded)
            {
                platform.transform.rotation = Quaternion.Lerp(platform.transform.rotation,
                 _intialPlatformRot, platformRoundEndResetSpeed * Time.deltaTime);
            }
            else
            {
                platform.transform.rotation = Quaternion.Lerp(platform.transform.rotation,
                _intialPlatformRot, platformResetSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Chooses a Random Enum Type;
        /// </summary>
        /// <typeparam name="T"> Needs an Enum Value; </typeparam>
        /// <returns> Current Enum Type; </returns>
        static T GetRandomEnum<T>()
        {
            System.Array A = System.Enum.GetValues(typeof(T));
            T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }
        #endregion

        #endregion

        #region Coroutines
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
            PlayerInputManager.instance.enabled = false;
        }

        /// <summary>
        /// Ends one round with a Delay;
        /// Resets the state game back to the initial state;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator EndRoundPointDelay()
        {
            yield return new WaitForSeconds(endRoundDelay);

            for (int i = 0; i < _playersBall.Count; i++)
            {
                _playersBall[i].transform.position = playerSpawnPos.position;
                _playersBall[i].gameObject.SetActive(true);
            }

            OpenWalls();
            gmData.ChangeGameState("Game");
        }

        /// <summary>
        /// Reset platform with delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator ResetRotatePlatformDelay()
        {
            _currPlatformRotateType = PlatformRotateType.Original;
            yield return new WaitForSeconds(resetPlatformTime);
            _currPlatformRotateType = GetRandomEnum<PlatformRotateType>();
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
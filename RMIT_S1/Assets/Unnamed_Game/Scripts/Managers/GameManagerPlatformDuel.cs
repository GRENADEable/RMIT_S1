using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
        [Tooltip("Starting Round Timer Text")]
        private TextMeshProUGUI startingRoundTimerText = default;

        [SerializeField]
        [Tooltip("Starting Round Timer Text")]
        private TextMeshProUGUI gameRoundTimerText = default;

        [SerializeField]
        [Tooltip("Popup Obsatcle Text")]
        private TextMeshProUGUI popupObstacleText = default;

        [Tooltip("Menu Button in an Array that will be used to disable them when clicking on other Buttons")]
        [SerializeField]
        private Button[] menuButtons;

        [SerializeField]
        [Tooltip("All the first button that the Event System will highlight")]
        private GameObject[] firstSelectedButtons = default;

        [SerializeField]
        [Tooltip("Timer Panel")]
        private GameObject timerPanel = default;

        [SerializeField]
        [Tooltip("HUD Panel")]
        private GameObject hudPanel = default;

        [SerializeField]
        [Tooltip("Pause Panel")]
        private GameObject pausePanel = default;

        [SerializeField]
        [Tooltip("Player UI GameObject Prefab")]
        private GameObject playerScorePrefab = default;

        [SerializeField]
        [Tooltip("Fade panel Animation Component")]
        private Animator fadeBG = default;

        [SerializeField]
        [Tooltip("Popup GameObject")]
        private Animator popupObstacleAreaAnim = default;

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
        [Tooltip("Platform Anim Controller")]
        private Animator platformWallsAnim = default;

        [SerializeField]
        [Tooltip("Player Death Box Col")]
        private GameObject playerDeathBox = default;
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

        #region Game Timers
        [Space, Header("Game Timers")]
        [SerializeField]
        [Tooltip("Starting Round Time")]
        private float startingGameRoundTimer = default;

        [SerializeField]
        [Tooltip("End Round Delay")]
        private float endRoundDelayTimer = default;

        [SerializeField]
        [Tooltip("Min Random value for switching controls")]
        private float minValSwitchControls = default;

        [SerializeField]
        [Tooltip("Min Random value for toggling jump controls")]
        private float minValJumpControls = default;
        #endregion

        #region Events Bool
        public delegate void SendEventsBool(bool isSwitched);
        /// <summary>
        /// Event sent from GameManagerPlatformDuel to PlayerControllerBall Script;
        /// Inverts the Player Controls;
        /// </summary>
        public static event SendEventsBool OnControlsReversed;

        /// <summary>
        /// Event sent from GameManagerPlatformDuel to PlayerControllerBall Script;
        /// Stops the player from Jumping;
        /// </summary>
        public static event SendEventsBool OnControlsJump;
        #endregion

        #endregion

        #region Private Variables

        #region UI

        #endregion

        #region Game
        [Header("Game")]
        private bool _isSwitchingControls = default;
        private bool _isJumpToggle = default;
        private List<PlayerControllerBall> _playersBall = new List<PlayerControllerBall>();
        private List<PlayerScore> _playersScore = new List<PlayerScore>();
        public int PlayerNo { get => _currPlayerNo; set => _currPlayerNo = value; }
        private int _currPlayerNo = default;
        private int _totalPlayerNo = default;
        #endregion

        #region Platform
        [Header("Platform")]
        private float _currPlatformRotTime = default;
        private Quaternion _intialPlatformRot = default;
        private enum PlatformRotateType { Original, RotateX, MinusRotateX, RotateY, MinusRotateY, RotateZ, MinusRotateZ };
        private PlatformRotateType _currPlatformRotateType = PlatformRotateType.RotateY;
        #endregion

        #region Game Timers
        [Header("Game Timers")]
        [SerializeField] private float _currGameRoundTime = default;
        [SerializeField] private float _switchControlTimer = default;
        [SerializeField] private float _jumpControlTimer = default;
        #endregion

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            PlayerControllerBall.OnPlayerIntialised += OnPlayerIntialisedEventReceived;
            PlayerControllerBall.OnPlayerFall += OnPlayerFallEventReceived;
            PlayerControllerBall.OnGamePaused += OnGamePausedEventReceived;
        }

        void OnDisable()
        {
            PlayerControllerBall.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
            PlayerControllerBall.OnPlayerFall -= OnPlayerFallEventReceived;
            PlayerControllerBall.OnGamePaused -= OnGamePausedEventReceived;
        }

        void OnDestroy()
        {
            PlayerControllerBall.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
            PlayerControllerBall.OnPlayerFall -= OnPlayerFallEventReceived;
            PlayerControllerBall.OnGamePaused -= OnGamePausedEventReceived;
        }
        #endregion

        void Start()
        {
            SetRoundTimers();

            _intialPlatformRot = platform.transform.rotation;
            _currPlatformRotateType = GetRandomEnum<PlatformRotateType>();
            _isJumpToggle = true;

            gmData.ChangeGameState("Intro");
            fadeBG.Play("Fade_In");

            if (isCursorDisabled)
                gmData.DisableCursor();
        }

        void Update()
        {
            // Testing Control Switch;
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    _isSwitchingControls = !_isSwitchingControls;
            //    _isJumpToggle = !_isJumpToggle;
            //    OnControlsReversed?.Invoke(_isSwitchingControls);
            //    OnControlsJump?.Invoke(!_isJumpToggle);
            //}

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

        #region UI
        /// <summary>
        /// Shows a popup Text with a parameter string;
        /// </summary>
        /// <param name="popupText"> What message to show on the text; </param>
        void PopupText(string popupText)
        {
            popupObstacleText.text = popupText;
            popupObstacleAreaAnim.Play("Pop_Obstacle_Anim");
        }

        #region Buttons
        /// <summary>
        /// Function tied with Resume_Button Button;
        /// Resumes the Game;
        /// </summary>
        public void OnClick_Resume()
        {
            EventSystem.current.SetSelectedGameObject(null);
            pausePanel.SetActive(false);
            hudPanel.SetActive(true);
            gmData.ChangeGameState("Game");
            gmData.TogglePause(false);
            gmData.DisableCursor();
        }

        /// <summary>
        /// Function tied with Restart_Button Button;
        /// Restarts the game with a delay;
        /// </summary>
        public void OnClick_Restart() => StartCoroutine(RestartDelay());

        /// <summary>
        /// Button tied with Menu_Button;
        /// Goes to the Menu with a delay;
        /// </summary>
        public void OnClick_Menu() => StartCoroutine(MenuDelay());

        /// <summary>
        /// Button tied with Quit_Button;
        /// Quits the Game
        /// </summary>
        public void OnClick_Quit() => StartCoroutine(QuitGameDelay());

        /// <summary>
        /// Tied to any UI Butttons;
        /// It will hightlight the button so that the user can navigate through the UI properly;
        /// </summary>
        /// <param name="index"> Which Button to highlight from th Array; </param>
        public void OnClick_HighlightedButton(int index)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButtons[index]);
        }

        /// <summary>
        /// Tied to the UI Buttons;
        /// All the buttons added in the Array gets disabled;
        /// </summary>
        public void OnClick_DisableButtons()
        {
            for (int i = 0; i < menuButtons.Length; i++)
                menuButtons[i].interactable = false;
        }
        #endregion

        #endregion

        #region Game
        /// <summary>
        /// Sets the UI of the player depending on how many players joined;
        /// </summary>
        void SetPlayersUI()
        {
            int playerIndex = 0;

            for (int i = 0; i < _playersBall.Count; i++)
            {
                GameObject plyScore = Instantiate(playerScorePrefab, playerScorePos.position, Quaternion.identity, playerScorePos);
                plyScore.name = playerVisData[i].playerName;

                _playersScore.Add(plyScore.GetComponent<PlayerScore>());
                _playersScore[i].PlayerName = playerVisData[i].playerName;
                _playersScore[i].PlayerPointIndex = playerIndex;

                playerIndex++;
            }

            hudPanel.SetActive(true);
            platformWallsAnim.Play("Platform_Duel_Wall_Open_Anim");
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

            //if (_currGameRoundTime >= _reverseControlOnTimer && _currGameRoundTime <= _reverseControlOnTimer)
            //    Debug.Log("Reversed Controls");

            //if (_currGameRoundTime <= _reverseControlOffTimer)
            //    Debug.Log("Normal Controls");
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
            platformWallsAnim.Play("Platform_Duel_Wall_Close_Anim");
            gmData.ChangeGameState("Starting");
            playerDeathBox.SetActive(false);
            StartCoroutine(EndRoundPointDelay());
        }

        /// <summary>
        /// Continues the game after the reset;
        /// </summary>
        void OpenWalls()
        {
            platformWallsAnim.Play("Platform_Duel_Wall_Open_Anim");
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

                case PlatformRotateType.MinusRotateX:
                    platform.transform.Rotate(-Vector3.right, platformRotSpeed * Time.deltaTime);
                    break;

                case PlatformRotateType.MinusRotateY:
                    platform.transform.Rotate(-Vector3.up, platformRotSpeed * Time.deltaTime);
                    break;

                case PlatformRotateType.MinusRotateZ:
                    platform.transform.Rotate(-Vector3.forward, platformRotSpeed * Time.deltaTime);
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

        #region Game Timers
        /// <summary>
        /// Intialises the round timers with the starting values;
        /// </summary>
        void SetRoundTimers()
        {
            _currGameRoundTime = startingGameRoundTimer;
            _currPlatformRotTime = startingPlatformRotatingTimer;
            gameRoundTimerText.text = startingGameRoundTimer.ToString();

            //_reverseControlOnTimer = Random.Range(startingGameRoundTimer / 2, startingGameRoundTimer);
            //_reverseControlOffTimer = Random.Range(0, startingGameRoundTimer / 2);
        }

        /// <summary>
        /// Switches the contorls of the player when the game is running;
        /// Shows the UI text of when the controls are switched;
        /// </summary>
        void SwitchControls()
        {
            _isSwitchingControls = !_isSwitchingControls;
            OnControlsReversed?.Invoke(_isSwitchingControls);

            if (_isSwitchingControls)
                PopupText("Switched Controls");
            else
                PopupText("Normal Controls");
        }

        /// <summary>
        /// Enables/Disables the player jump when the game is running;
        /// Shows the UI text of when the jump is enabled/disabled;
        /// </summary>
        void ToggleJumpControl()
        {
            _isJumpToggle = !_isJumpToggle;
            OnControlsJump?.Invoke(_isJumpToggle);

            if (_isJumpToggle)
                PopupText("Enabled Jump");
            else
                PopupText("Disabled Jump");
        }
        #endregion

        #endregion

        #region Coroutines

        #region Game
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
            StartCoroutine(SwitchControlsDelay());
            StartCoroutine(JumpControlsDelay());
        }

        /// <summary>
        /// Ends one round with a Delay;
        /// Resets the state game back to the initial state;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator EndRoundPointDelay()
        {
            yield return new WaitForSeconds(endRoundDelayTimer);

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

        /// <summary>
        /// Switches controls with delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator SwitchControlsDelay()
        {
            _switchControlTimer = Random.Range(minValSwitchControls, startingGameRoundTimer);
            yield return new WaitForSeconds(_switchControlTimer);
            SwitchControls();
            _switchControlTimer = Random.Range(minValSwitchControls, startingGameRoundTimer / 2);
            yield return new WaitForSeconds(_switchControlTimer);
            SwitchControls();
            StartCoroutine(SwitchControlsDelay());
        }

        /// <summary>
        /// Toggles Jump controls with delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator JumpControlsDelay()
        {
            _jumpControlTimer = Random.Range(minValJumpControls, startingGameRoundTimer / 2);
            yield return new WaitForSeconds(_jumpControlTimer);
            ToggleJumpControl();
            _jumpControlTimer = Random.Range(minValJumpControls, startingGameRoundTimer / 2);
            yield return new WaitForSeconds(_jumpControlTimer);
            ToggleJumpControl();
            StartCoroutine(JumpControlsDelay());
        }
        #endregion

        #region Buttons
        /// <summary>
        /// Restarts the game with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator RestartDelay()
        {
            gmData.TogglePause(false);
            fadeBG.Play("Fade_Out");
            yield return new WaitForSeconds(0.5f);
            gmData.ChangeLevel(Application.loadedLevel);
        }

        /// <summary>
        /// Goes to Menu with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator MenuDelay()
        {
            gmData.TogglePause(false);
            fadeBG.Play("Fade_Out");
            yield return new WaitForSeconds(0.5f);
            gmData.ChangeLevel(0);
        }

        /// <summary>
        /// Quits the game with a Delay;
        /// </summary>
        /// <returns> Float Delay </returns>
        IEnumerator QuitGameDelay()
        {
            gmData.TogglePause(false);
            fadeBG.Play("Fade_Out");
            yield return new WaitForSeconds(0.5f);
            gmData.QuitGame();
        }
        #endregion

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
                PlayerNo = _totalPlayerNo;
                EndRoundWithPoint();
                //StartCoroutine(SwitchControlsDelay());
            }
        }

        /// <summary>
        /// Subbed to Event from PlayerControllerBall Script;
        /// Pauses the game;
        /// </summary>
        void OnGamePausedEventReceived()
        {
            gmData.ChangeGameState("Paused");
            OnClick_HighlightedButton(0);
            pausePanel.SetActive(true);
            hudPanel.SetActive(false);
            gmData.EnableCursor();
            gmData.TogglePause(true);
        }
        #endregion
    }
}
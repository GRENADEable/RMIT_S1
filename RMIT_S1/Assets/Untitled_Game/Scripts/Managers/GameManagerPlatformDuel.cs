using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
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
        [Tooltip("Player Input Manager")]
        private PlayerInputManager plyInputManager = default;

        [SerializeField]
        [Tooltip("Do you want to disable Cursor?")]
        private bool isCursorDisabled = default;
        #endregion

        #region UI
        [Space, Header("UI")]
        //[SerializeField]
        //[Tooltip("Controls Text")]
        //private TextMeshProUGUI controlsCountdownText = default;

        [SerializeField]
        [Tooltip("Starting Round Timer Text")]
        private TextMeshProUGUI startingRoundTimerText = default;

        [SerializeField]
        [Tooltip("Starting Round Timer Text")]
        private TextMeshProUGUI gameRoundTimerText = default;

        [SerializeField]
        [Tooltip("Popup Obsatcle Text")]
        private TextMeshProUGUI popupObstacleText = default;

        [SerializeField]
        [Tooltip("Switch Controls Text")]
        private TextMeshProUGUI switchControlsText = default;

        [SerializeField]
        [Tooltip("Menu Button in an Array that will be used to disable them when clicking on other Buttons")]
        private Button[] menuButtons;

        [SerializeField]
        [Tooltip("Switched Controls Image Component")]
        private Image switchedControlsImg = default;

        [SerializeField]
        [Tooltip("Jump Controls Image Component")]
        private Image jumpControlsImg = default;

        [SerializeField]
        [Tooltip("Obstacle Sprites")]
        private Sprite[] obstacleSprites = default;
        #endregion

        #region Animators
        [Space, Header("Animators")]
        [SerializeField]
        [Tooltip("Intro Timeline")]
        private PlayableDirector introTimeline = default;

        [SerializeField]
        [Tooltip("Fade panel Animation Component")]
        private Animator fadeBG = default;

        [SerializeField]
        [Tooltip("Popup GameObject")]
        private Animator popupObstacleAreaAnim = default;

        [SerializeField]
        [Tooltip("Platform Anim Controller")]
        private Animator platformWallsAnim = default;
        #endregion

        #region GameObjects
        [Space, Header("GameObjects")]
        [SerializeField]
        [Tooltip("Intro Virtual Cams GameObject")]
        private GameObject introVCams = default;

        [SerializeField]
        [Tooltip("All the first button that the Event System will highlight")]
        private GameObject[] firstSelectedButtons = default;

        [SerializeField]
        [Tooltip("Player float Name Prefab")]
        private GameObject playerFloatPrefab = default;

        [SerializeField]
        [Tooltip("Controls Panel")]
        private GameObject controlsPanel = default;

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
        [Tooltip("Switch Controls Panel")]
        private GameObject switchControlsPanel = default;

        [SerializeField]
        [Tooltip("Player Death Box Col")]
        private GameObject playerDeathBox = default;

        [SerializeField]
        [Tooltip("Platform GameObject")]
        private GameObject platform = default;
        #endregion

        #region Transforms
        [Space, Header("Transforms")]
        [SerializeField]
        [Tooltip("Player Score Spawn Pos")]
        private Transform playerScorePos = default;

        [SerializeField]
        [Tooltip("Player Spawn Pos")]
        private Transform playerSpawnPos = default;
        #endregion

        #region Ints
        [Space, Header("Ints")]
        [SerializeField]
        [Tooltip("Player Score Increment")]
        private int playerScoreIncrement = 1;

        [SerializeField]
        [Tooltip("Intial Starting Players")]
        private int playerCountToStartMatch = default;
        #endregion

        #region Flaots
        [Space, Header("Floats")]
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

        [SerializeField]
        [Tooltip("Starting Round Time")]
        private float startingGameRoundTimer = default;

        [SerializeField]
        [Tooltip("End Round Timer")]
        private float endRoundDelayTimer = default;
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

        #region Events Void
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from GameManagerPlatformDuel to CrowdManager Scripts;
        /// Stops the current Anim and players the PogAnim;
        /// </summary>
        public static event SendEvents OnPogAnim;
        #endregion

        #endregion

        #region Private Variables

        #region Lists
        [Header("Lists")]
        private List<PlayerControllerBall> _playersBall = new List<PlayerControllerBall>();
        private List<PlayerScore> _playersScore = new List<PlayerScore>();
        private List<PlayerFloatingName> _playersFloatName = new List<PlayerFloatingName>();
        #endregion

        #region Int
        [Header("Int")]
        private int _totalPlayerNo = default;
        public int PlayerNo { get => _currPlayerNo; set => _currPlayerNo = value; }
        private int _currPlayerNo = default;
        #endregion

        #region Flaots
        [Header("Flaots")]
        private float _currPlatformRotTime = default;
        private float _currGameRoundTime = default;
        private float _currControlsChangeTimer = default;

        #endregion

        #region Bools
        [Header("Bools")]
        private bool _isObstacleEventSent = default;
        #endregion

        #region Vector3 & Quaternions
        [Header("Vector3 & Quaternions")]
        private Quaternion _intialPlatformRot = default;
        private Vector3 _intialPlatformScale = default;
        #endregion

        #region Enums
        [Header("Enums")]
        private ObstacleType _currObstacleType = ObstacleType.None;
        private enum ObstacleType { All, DisabledJump, SwitchedControls, None };

        private enum PlatformRotateType { Original, RotateX, MinusRotateX, RotateY, MinusRotateY, RotateZ, MinusRotateZ };
        private PlatformRotateType _currPlatformRotateType = PlatformRotateType.RotateY;
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
            //StartCoroutine(ControlsDelay());

            SetRoundTimers();

            _intialPlatformRot = platform.transform.rotation;
            _intialPlatformScale = platform.transform.localScale;
            _currPlatformRotateType = GetRandomEnum<PlatformRotateType>();

            gmData.ChangeGameState("Intro");

            fadeBG.Play("Fade_In");

            if (isCursorDisabled)
                gmData.DisableCursor();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                _currControlsChangeTimer = 2;

            if (Input.GetKeyDown(KeyCode.T))
                _currGameRoundTime = 2;

            if (gmData.currState == GameManagerDataMiniGame.GameState.Game)
            {
                RoundTimer();
                RotatePlatform();
                ShrinkPlatform();
                ObstacleManager();
            }

            if (gmData.currState == GameManagerDataMiniGame.GameState.Starting)
                ResetPlatform(true);
        }
        #endregion

        #region My Functions

        #region Timeline
        /// <summary>
        /// Tied to Signal Emitter from Timeline;
        /// Switches from cutscene to gameplay;
        /// </summary>
        public void OnIntroEnd()
        {
            introTimeline.gameObject.SetActive(false);
            introVCams.SetActive(false);
            fadeBG.Play("Fade_In");
        }
        #endregion

        #region UI
        public void OnClick_ContinueToGame()
        {
            plyInputManager.enabled = true;
            controlsPanel.SetActive(false);
            introTimeline.Play();
            EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        /// Shows a popup Text with a parameter string;
        /// </summary>
        /// <param name="popupText"> What message to show on the text; </param>
        void PopupText(string popupText)
        {
            popupObstacleText.text = popupText;
            popupObstacleAreaAnim.Play("Pop_Obstacle_Anim");
        }

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
                _playersScore[i].PlayerImg.color = playerVisData[i].playerColour;

                playerIndex++;
            }

            for (int i = 0; i < _playersFloatName.Count; i++)
                Destroy(_playersFloatName[i].gameObject);

            _playersFloatName.Clear();

            hudPanel.SetActive(true);
            platformWallsAnim.Play("Platform_Duel_Wall_Open_Anim");
            _totalPlayerNo = PlayerNo;
            gmData.ChangeGameState("Game");
        }

        #region Buttons
        /// <summary>
        /// Function tied with Resume_Button Button;
        /// Resumes the Game;
        /// </summary>
        public void OnClick_Resume()
        {
            EventSystem.current.SetSelectedGameObject(null);

            if (gmData.currState == GameManagerDataMiniGame.GameState.Paused)
            {
                gmData.ChangeGameState("Game");
                hudPanel.SetActive(true);
            }

            pausePanel.SetActive(false);
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
        /// If one player stands, they win a point;
        /// </summary>
        void EndRoundWithPoint()
        {
            CloseWalls();
            Debug.Log("Round Ended with Point");

            for (int i = 0; i < _playersBall.Count; i++)
            {
                if (_playersBall[i].gameObject.activeInHierarchy)
                {
                    _playersScore[_playersBall[i].PlayerIndex].UpdateScore(playerScoreIncrement);
                    PopupText($"{_playersScore[i].PlayerName} Won");
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
            PopupText("Nobody Won");
            Debug.Log("Round Ended without Point");

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
            jumpControlsImg.sprite = obstacleSprites[1];
            switchedControlsImg.gameObject.SetActive(false);

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
                    ResetPlatform(false);
                    break;

                default:
                    break;
            }
        }

        void ShrinkPlatform()
        {
            float shrinkVal = Mathf.InverseLerp(0, startingGameRoundTimer, _currGameRoundTime);
            platform.transform.localScale = new Vector3(shrinkVal, platform.transform.localScale.y, shrinkVal);
        }

        /// <summary>
        /// Resets the Rotating Platform when round Ends;
        /// </summary>
        /// <param name="isRoundEnded"> If true, reset it quickly, else reset it slowly; </param>
        void ResetPlatform(bool isRoundEnded)
        {
            if (isRoundEnded)
            {
                platform.transform.rotation = Quaternion.Lerp(platform.transform.rotation,
                 _intialPlatformRot, platformRoundEndResetSpeed * Time.deltaTime);

                platform.transform.localScale = Vector3.Lerp(platform.transform.localScale,
                _intialPlatformScale, platformRoundEndResetSpeed * Time.deltaTime);
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
        /// Intialises the round timers with the starting values;
        /// </summary>
        void SetRoundTimers()
        {
            _currGameRoundTime = startingGameRoundTimer;
            _currPlatformRotTime = startingPlatformRotatingTimer;
            _currControlsChangeTimer = Random.Range(10f, startingGameRoundTimer / 2);
            gameRoundTimerText.text = startingGameRoundTimer.ToString();
        }
        #endregion

        #region Obstacles
        /// <summary>
        /// Changes the controls after the timer ends;
        /// </summary>
        void ChangeControls()
        {
            _currObstacleType = GetRandomEnum<ObstacleType>();
            _currControlsChangeTimer = Random.Range(10f, startingGameRoundTimer / 2);
            //PopupText("Controls Changed");
            _isObstacleEventSent = true;
        }

        /// <summary>
        /// Switches the contorls of the player when the game is running;
        /// Shows the UI text of when the controls are switched;
        /// </summary>
        void SwitchControls(bool isSwitching)
        {
            OnControlsReversed?.Invoke(isSwitching);

            if (isSwitching)
            {
                switchedControlsImg.gameObject.SetActive(true);
                Debug.Log("Switched Controls");
            }
            else
            {
                switchedControlsImg.gameObject.SetActive(false);
                Debug.Log("Normal Controls");
            }
        }

        /// <summary>
        /// Enables/Disables the player jump when the game is running;
        /// Shows the UI text of when the jump is enabled/disabled;
        /// </summary>
        void JumpControls(bool isJumping)
        {
            OnControlsJump?.Invoke(isJumping);

            if (isJumping)
            {
                jumpControlsImg.sprite = obstacleSprites[1];
                Debug.Log("Enabled Jump");
            }
            else
            {
                jumpControlsImg.sprite = obstacleSprites[2];
                Debug.Log("Disabled Jump");
            }
        }

        /// <summary>
        /// Manages the obstacles of this match such as disabled jump and switched controls;
        /// </summary>
        void ObstacleManager()
        {
            _currControlsChangeTimer -= Time.deltaTime;
            switchControlsText.text = _currControlsChangeTimer.ToString("f1");

            if (_currControlsChangeTimer <= 3f)
                switchControlsPanel.SetActive(true);
            else
                switchControlsPanel.SetActive(false);

            if (_currControlsChangeTimer <= 0f)
            {
                switchControlsPanel.SetActive(false);
                ChangeControls();
            }

            switch (_currObstacleType)
            {
                case ObstacleType.All:
                    if (_isObstacleEventSent)
                    {
                        _isObstacleEventSent = false;
                        JumpControls(false);
                        SwitchControls(true);
                        PopupText("Controls Switched, No Jump");
                    }
                    break;

                case ObstacleType.DisabledJump:
                    if (_isObstacleEventSent)
                    {
                        _isObstacleEventSent = false;
                        JumpControls(false);
                        SwitchControls(false);
                        PopupText("Controls Normal, No Jump");
                    }
                    break;

                case ObstacleType.SwitchedControls:
                    if (_isObstacleEventSent)
                    {
                        _isObstacleEventSent = false;
                        JumpControls(true);
                        SwitchControls(true);
                        PopupText("Controls Switched, Jump");
                    }
                    break;

                case ObstacleType.None:
                    if (_isObstacleEventSent)
                    {
                        _isObstacleEventSent = false;
                        JumpControls(true);
                        SwitchControls(false);
                        PopupText("Controls Normals, Jump");
                    }
                    break;

                default:
                    break;
            }
        }
        #endregion

        #endregion

        #region Coroutines

        #region Game
        //IEnumerator ControlsDelay()
        //{
        //    controlsCountdownText.text = "6";
        //    yield return new WaitForSeconds(1);
        //    controlsCountdownText.text = "5";
        //    yield return new WaitForSeconds(1);
        //    controlsCountdownText.text = "4";
        //    yield return new WaitForSeconds(1);
        //    controlsCountdownText.text = "3";
        //    yield return new WaitForSeconds(1);
        //    controlsCountdownText.text = "2";
        //    yield return new WaitForSeconds(1);
        //    controlsCountdownText.text = "1";
        //    yield return new WaitForSeconds(1);
        //    controlsCountdownText.text = "0";
        //    yield return new WaitForSeconds(1);
        //    plyInputManager.enabled = true;
        //    controlsPanel.SetActive(false);
        //    introTimeline.Play();
        //}

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
            plyInputManager.enabled = false;
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
        #endregion

        #region Buttons
        /// <summary>
        /// Restarts the game with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator RestartDelay()
        {
            gmData.TogglePause(false);
            introTimeline.gameObject.SetActive(false);
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
            introTimeline.gameObject.SetActive(false);
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
            introTimeline.gameObject.SetActive(false);
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
            // Sets up the ball variables;
            _playersBall.Add(plyBall);
            _playersBall[PlayerNo].name = $"{playerVisData[PlayerNo].playerName}";
            _playersBall[PlayerNo].GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", playerVisData[PlayerNo].playerColour);
            _playersBall[PlayerNo].transform.position = playerSpawnPos.position;
            _playersBall[PlayerNo].PlayerIndex = PlayerNo;

            // Sets Up Player floating Names;
            GameObject plyFloatName = Instantiate(playerFloatPrefab, playerSpawnPos.position, Quaternion.identity, plyBall.transform);
            PlayerFloatingName floatingName = plyFloatName.GetComponent<PlayerFloatingName>();
            _playersFloatName.Add(floatingName);
            floatingName.FloatingNameText.text = playerVisData[PlayerNo].playerName;
            floatingName.FollowPos = plyBall.gameObject.transform;
            floatingName.name = $"{playerVisData[PlayerNo].playerName}_Name_Canvas_World";

            PlayerNo++;

            if (PlayerNo == playerCountToStartMatch)
            {
                gmData.ChangeGameState("Starting");
                OnIntroEnd();
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
                OnPogAnim?.Invoke();
            }
        }

        /// <summary>
        /// Subbed to Event from PlayerControllerBall Script;
        /// Pauses the game;
        /// </summary>
        void OnGamePausedEventReceived()
        {
            if (gmData.currState == GameManagerDataMiniGame.GameState.Game)
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
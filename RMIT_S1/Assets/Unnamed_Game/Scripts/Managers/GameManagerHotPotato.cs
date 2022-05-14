using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Khatim_F2
{
    public class GameManagerHotPotato : MonoBehaviour
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

        #region UI
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

        [Tooltip("Menu Button in an Array that will be used to disable them when clicking on other Buttons")]
        [SerializeField]
        private Button[] menuButtons;

        [SerializeField]
        [Tooltip("Fade panel Animation Component")]
        private Animator fadeBG = default;

        [SerializeField]
        [Tooltip("Popup GameObject")]
        private Animator popupObstacleAreaAnim = default;
        #endregion

        #region GameObjects
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
        [Tooltip("Switch Controls Panel")]
        private GameObject switchControlsPanel = default;
        #endregion

        #region Transforms
        [SerializeField]
        [Tooltip("Player Score Spawn Pos")]
        private Transform playerScorePos = default;
        #endregion

        #endregion

        #region Game
        [Space, Header("Game")]
        [SerializeField]
        [Tooltip("Player Spawn Pos")]
        private GameObject playerSpawnPos = default;

        [SerializeField]
        [Tooltip("Player Death Box Col")]
        private GameObject playerDeathBox = default;

        [SerializeField]
        [Tooltip("Player Score Increment")]
        private int playerScoreIncrement = 1;

        [SerializeField]
        [Tooltip("Intial Starting Players")]
        private int playerCountToStartMatch = default;
        #endregion

        #region Platform

        #endregion

        #region Game Timers
        [Space, Header("Game Timers")]
        [SerializeField]
        [Tooltip("Starting Round Time")]
        private float startingGameRoundTimer = default;

        [SerializeField]
        [Tooltip("End Round Timer")]
        private float endRoundDelayTimer = default;
        #endregion

        #endregion

        #region Private Variables

        #region UI

        #endregion

        #region Game
        [Header("Game")]
        private List<PlayerControllerCapsule> _playersCapsule = new List<PlayerControllerCapsule>();
        [SerializeField] private List<CharacterController> _playersCharController = new List<CharacterController>();
        [SerializeField] private List<CapsuleCollider> _playersCapsuleCol = new List<CapsuleCollider>();
        private List<PlayerScore> _playersScore = new List<PlayerScore>();
        [SerializeField] private List<PlayerSpawns> _playerSpawns = new List<PlayerSpawns>();
        [SerializeField] private int spawnIndex = default;

        public int PlayerNo { get => _currPlayerNo; set => _currPlayerNo = value; }
        private int _currPlayerNo = default;
        private int _totalPlayerNo = default;
        #endregion

        #region Platform
        [Header("Platform")]
        private float _currPlatformRotTime = default;
        private Quaternion _intialPlatformRot = default;
        private Vector3 _intialPlatformScale = default;

        private enum PlatformRotateType { Original, RotateX, MinusRotateX, RotateY, MinusRotateY, RotateZ, MinusRotateZ };
        private PlatformRotateType _currPlatformRotateType = PlatformRotateType.RotateY;
        #endregion

        #region Game Timers
        [Header("Game Timers")]
        private ObstacleType _currObstacleType = ObstacleType.None;
        private enum ObstacleType { All, DisabledJump, SwitchedControls, None };
        private bool _isObstacleEventSent = default;

        [SerializeField] private float _currGameRoundTime = default;
        [SerializeField] private float _currControlsChangeTimer = default;
        #endregion

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            PlayerControllerCapsule.OnPlayerIntialised += OnPlayerIntialisedEventReceived;
            PlayerControllerCapsule.OnPlayerFall += OnPlayerFallEventReceived;
            PlayerControllerCapsule.OnGamePaused += OnGamePausedEventReceived;
        }

        void OnDisable()
        {
            PlayerControllerCapsule.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
            PlayerControllerCapsule.OnPlayerFall -= OnPlayerFallEventReceived;
            PlayerControllerCapsule.OnGamePaused -= OnGamePausedEventReceived;
        }

        void OnDestroy()
        {
            PlayerControllerCapsule.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
            PlayerControllerCapsule.OnPlayerFall -= OnPlayerFallEventReceived;
            PlayerControllerCapsule.OnGamePaused -= OnGamePausedEventReceived;
        }
        #endregion

        void Start()
        {
            SetRoundTimers();
            GetPlayerSpawns();

            _currPlatformRotateType = GetRandomEnum<PlatformRotateType>();

            gmData.ChangeGameState("Intro");
            fadeBG.Play("Fade_In");

            if (isCursorDisabled)
                gmData.DisableCursor();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                _currGameRoundTime = 2;

            if (gmData.currState == GameManagerDataMiniGame.GameState.Game)
            {
                RoundTimer();
                ObstacleManager();
            }
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
        /// Sets the UI of the player depending on how many players joined;
        /// </summary>
        void SetPlayersUI()
        {
            int playerIndex = 0;

            for (int i = 0; i < _playersCapsule.Count; i++)
            {
                GameObject plyScore = Instantiate(playerScorePrefab, playerScorePos.position, Quaternion.identity, playerScorePos);
                plyScore.name = playerVisData[i].playerName;

                _playersScore.Add(plyScore.GetComponent<PlayerScore>());
                _playersScore[i].PlayerName = playerVisData[i].playerName;
                _playersScore[i].PlayerPointIndex = playerIndex;

                playerIndex++;
            }

            hudPanel.SetActive(true);
            _totalPlayerNo = PlayerNo;
            gmData.ChangeGameState("Game");
        }

        void GetPlayerSpawns()
        {
            PlayerSpawns[] playerSpawns;
            playerSpawns = playerSpawnPos.GetComponentsInChildren<PlayerSpawns>();

            for (int i = 0; i < playerSpawns.Length; i++)
                _playerSpawns.Add(playerSpawns[i]);
        }

        Transform SetPlayerSpawns()
        {
            spawnIndex = Random.Range(0, _playerSpawns.Count);
            return _playerSpawns[spawnIndex].transform;
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
            }
        }
        #endregion

        #region Platform
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
            _currControlsChangeTimer = Random.Range(10f, startingGameRoundTimer / 2);
            gameRoundTimerText.text = startingGameRoundTimer.ToString();
        }

        /// <summary>
        /// Changes the controls after the timer ends;
        /// </summary>
        void ChangeControls()
        {
            _currObstacleType = GetRandomEnum<ObstacleType>();
            _currControlsChangeTimer = Random.Range(10f, startingGameRoundTimer / 2);
            PopupText("Controls Changed");
            _isObstacleEventSent = true;
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

            if (_currControlsChangeTimer <= 0f)
            {
                switchControlsPanel.SetActive(false);
                ChangeControls();
            }

            switch (_currObstacleType)
            {
                case ObstacleType.All:

                    break;

                case ObstacleType.DisabledJump:

                    break;

                case ObstacleType.SwitchedControls:

                    break;

                case ObstacleType.None:

                    break;

                default:
                    break;
            }
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
        }

        /// <summary>
        /// Ends one round with a Delay;
        /// Resets the state game back to the initial state;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator EndRoundPointDelay()
        {
            yield return new WaitForSeconds(endRoundDelayTimer);

            for (int i = 0; i < _playersCapsule.Count; i++)
            {
                //_playersBall[i].transform.position = playerSpawnPos.position;
                _playersCapsule[i].gameObject.SetActive(true);
            }

            gmData.ChangeGameState("Game");
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
        void OnPlayerIntialisedEventReceived(PlayerControllerCapsule plyBall)
        {
            _playersCapsule.Add(plyBall);
            _playersCharController.Add(plyBall.GetComponent<CharacterController>());
            _playersCapsuleCol.Add(plyBall.GetComponent<CapsuleCollider>());

            _playersCapsule[PlayerNo].name = $"{playerVisData[PlayerNo].playerName}";
            _playersCapsule[PlayerNo].GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", playerVisData[PlayerNo].playerColour);
            _playersCapsule[PlayerNo].PlayerIndex = PlayerNo;

            _playersCapsule[PlayerNo].transform.position = SetPlayerSpawns().position;
            _playersCharController[PlayerNo].enabled = true;

            PlayerNo++;
            _playerSpawns.RemoveAt(spawnIndex);

            if (PlayerNo == playerCountToStartMatch)
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
            _playersCapsule[index].gameObject.SetActive(false);
            PlayerNo--;

            if (PlayerNo <= 1)
            {
                PlayerNo = _totalPlayerNo;
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
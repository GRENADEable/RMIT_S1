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

        [SerializeField]
        [Tooltip("Jump Controls Image Component")]
        private Image jumpControlsImg = default;

        [SerializeField]
        [Tooltip("Speed Controls Image Component")]
        private Image speedControlsImg = default;

        [SerializeField]
        [Tooltip("Obstacle Sprites")]
        private Sprite[] obstacleSprites = default;
        #endregion

        #region GameObjects
        [SerializeField]
        [Tooltip("All the first button that the Event System will highlight")]
        private GameObject[] firstSelectedButtons = default;

        [SerializeField]
        [Tooltip("Player float Name Prefab")]
        private GameObject playerFloatPrefab = default;

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
        [Tooltip("Switch Controls Panel")]
        private GameObject switchControlsPanel = default;
        #endregion

        #endregion

        #region Game
        [Space, Header("Game")]
        [SerializeField]
        [Tooltip("Player Spawn Pos")]
        private GameObject playerSpawnPos = default;

        [SerializeField]
        [Tooltip("Intial Starting Players")]
        private int playerCountToStartMatch = default;
        #endregion

        #region Game Timers
        [Space, Header("Game Timers")]
        [SerializeField]
        [Tooltip("Starting Round Time")]
        private float startingGameRoundTimer = default;
        #endregion

        #region Ocean
        [Space, Header("Ocean")]
        [SerializeField]
        [Tooltip("Ocean GameObject")]
        private GameObject oceanGround = default;

        [SerializeField]
        [Tooltip("Ocean raise Speed")]
        private float oceanRaiseSpeed = default;

        [SerializeField]
        [Tooltip("Ocean Reset Speed when round ends")]
        [Range(0f, 10f)]
        private float oceanRoundEndResetSpeed = default;

        [SerializeField]
        [Tooltip("Ocean end Position")]
        private Transform oceanEndPos = default;
        #endregion

        #region Events Bool
        public delegate void SendEventsBool(bool isSwitched);

        /// <summary>
        /// Event sent from GameManagerPlatformDuel to PlayerControllerCapsule Script;
        /// Stops the player from Jumping;
        /// </summary>
        public static event SendEventsBool OnControlsJump;

        /// <summary>
        /// Event sent from GameManagerPlatformDuel to PlayerControllerCapsule Script;
        /// Changes the player's speed;
        /// </summary>
        public static event SendEventsBool OnControlsSpeed;
        #endregion

        #region Events Int
        public delegate void SendEventsInt(int bombIndex);

        public static event SendEventsInt OnBombChoose;
        #endregion

        #endregion

        #region Private Variables

        #region Game
        [Header("Game")]
        private List<PlayerControllerCapsule> _playersController = new List<PlayerControllerCapsule>();
        private List<CharacterController> _playersCharController = new List<CharacterController>();
        private List<CapsuleCollider> _playersCol = new List<CapsuleCollider>();
        private List<PlayerSpawns> _playerSpawns = new List<PlayerSpawns>();
        private List<PlayerFloatingName> _playersFloatName = new List<PlayerFloatingName>();
        private int spawnIndex = default;

        public int PlayerNo { get => _currPlayerNo; set => _currPlayerNo = value; }
        private int _currPlayerNo = default;
        private int _currBombPlayerIndex = default;
        #endregion

        #region Game Timers
        [Header("Game Timers")]
        private ObstacleType _currObstacleType = ObstacleType.None;
        private enum ObstacleType { All, DisabledJump, SuperSpeed, None };
        private bool _isObstacleEventSent = default;

        private float _currGameRoundTime = default;
        private float _currControlsChangeTimer = default;
        #endregion

        #region Ocean
        [Header("Ocean")]
        private OceanType _currOceanType = OceanType.None;
        private enum OceanType { Raise, Hold, Reset, None };
        private Vector3 _intialOceanPos = default;
        #endregion

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            PlayerControllerCapsule.OnPlayerIntialised += OnPlayerIntialisedEventReceived;
            PlayerControllerCapsule.OnGamePaused += OnGamePausedEventReceived;
            PlayerControllerCapsule.OnPlayerPassBomb += OnPlayerPassBombEventReceived;
        }

        void OnDisable()
        {
            PlayerControllerCapsule.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
            PlayerControllerCapsule.OnGamePaused -= OnGamePausedEventReceived;
            PlayerControllerCapsule.OnPlayerPassBomb -= OnPlayerPassBombEventReceived;
        }

        void OnDestroy()
        {
            PlayerControllerCapsule.OnPlayerIntialised -= OnPlayerIntialisedEventReceived;
            PlayerControllerCapsule.OnGamePaused -= OnGamePausedEventReceived;
            PlayerControllerCapsule.OnPlayerPassBomb -= OnPlayerPassBombEventReceived;
        }
        #endregion

        void Start()
        {
            SetRoundTimers();
            GetPlayerSpawns();

            gmData.ChangeGameState("Intro");
            fadeBG.Play("Fade_In");

            _intialOceanPos = oceanGround.transform.position;

            if (isCursorDisabled)
                gmData.DisableCursor();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                _currGameRoundTime = 2;

            if (Input.GetKeyDown(KeyCode.Q))
                _currControlsChangeTimer = 2;

            if (gmData.currState == GameManagerDataMiniGame.GameState.Game)
            {
                RoundTimer();
                ObstacleManager();
            }

            OceanGround();
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
        /// Randomly chooses the player that will hold the bomb;
        /// </summary>
        void ChooseBombPlayer()
        {
            _currBombPlayerIndex = Random.Range(0, _playersController.Count);
            OnBombChoose?.Invoke(_currBombPlayerIndex);
        }

        void UpdatePlayerIndex()
        {
            int newIndex = 0;

            for (int i = 0; i < _playersController.Count; i++)
            {
                _playersController[i].PlayerIndex = newIndex;
                newIndex++;
            }
        }

        /// <summary>
        /// Gets all teh child spawnpoints and adds them to the list;
        /// </summary>
        void GetPlayerSpawns()
        {
            PlayerSpawns[] playerSpawns;
            playerSpawns = playerSpawnPos.GetComponentsInChildren<PlayerSpawns>();

            for (int i = 0; i < playerSpawns.Length; i++)
                _playerSpawns.Add(playerSpawns[i]);
        }

        /// <summary>
        /// Eliminates the current player that is holding the bomb;
        /// </summary>
        void EliminatePlayer()
        {
            PopupText($"Player {_currBombPlayerIndex + 1} Elimnated");

            _playersController[_currBombPlayerIndex].gameObject.SetActive(false);
            _playersController.RemoveAt(_currBombPlayerIndex);
            _playersCol.RemoveAt(_currBombPlayerIndex);

            gmData.ChangeGameState("Starting");

            PlayerNo--;

            StartCoroutine(ContinueMatchDelay());
            //Debug.Log("Round Ended");
        }

        /// <summary>
        /// Sets the random spawnpoints of the player
        /// </summary>
        /// <returns> Returns Transform; </returns>
        Transform SetPlayerSpawns()
        {
            spawnIndex = Random.Range(0, _playerSpawns.Count);
            return _playerSpawns[spawnIndex].transform;
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
            _currControlsChangeTimer = Random.Range(10f, startingGameRoundTimer / 2);
            gameRoundTimerText.text = startingGameRoundTimer.ToString();
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
                SetRoundTimers();
                EliminatePlayer();
                JumpControls(true);
                SpeedyControls(false);
            }
        }
        #endregion

        #region Game Obstacles
        /// <summary>
        /// Enables/Disables the player jump when the game is running;
        /// Shows the UI text of when the jump is enabled/disabled;
        /// </summary>
        void JumpControls(bool isJumping)
        {
            OnControlsJump?.Invoke(isJumping);

            if (isJumping)
            {
                jumpControlsImg.sprite = obstacleSprites[0];
                Debug.Log("Enabled Jump");
            }
            else
            {
                jumpControlsImg.sprite = obstacleSprites[1];
                Debug.Log("Disabled Jump");
            }
        }

        /// <summary>
        /// Enables/Disables the player jump when the game is running;
        /// Shows the UI text of when the jump is enabled/disabled;
        /// </summary>
        void SpeedyControls(bool isSpeeding)
        {
            OnControlsSpeed?.Invoke(isSpeeding);

            if (isSpeeding)
            {
                speedControlsImg.sprite = obstacleSprites[2];
                Debug.Log("Enabled Speeding");
            }
            else
            {
                speedControlsImg.sprite = obstacleSprites[3];
                Debug.Log("Disabled Speeding");
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
                        SpeedyControls(true);
                    }
                    break;

                case ObstacleType.DisabledJump:
                    if (_isObstacleEventSent)
                    {
                        _isObstacleEventSent = false;
                        JumpControls(false);
                        SpeedyControls(false);
                    }
                    break;

                case ObstacleType.SuperSpeed:
                    if (_isObstacleEventSent)
                    {
                        _isObstacleEventSent = false;
                        JumpControls(true);
                        SpeedyControls(true);
                    }
                    break;

                case ObstacleType.None:
                    if (_isObstacleEventSent)
                    {
                        _isObstacleEventSent = false;
                        JumpControls(true);
                        SpeedyControls(false);
                    }
                    break;

                default:
                    break;
            }
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

        void OceanGround()
        {
            switch (_currOceanType)
            {
                case OceanType.Raise:
                    oceanGround.transform.position = Vector3.MoveTowards(oceanGround.transform.position, oceanEndPos.position, oceanRaiseSpeed * Time.deltaTime);
                    break;

                case OceanType.Hold:
                    break;

                case OceanType.Reset:
                    oceanGround.transform.position = Vector3.MoveTowards(oceanGround.transform.position, _intialOceanPos, oceanRoundEndResetSpeed * Time.deltaTime);
                    break;

                case OceanType.None:
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

            ChooseBombPlayer();
            timerPanel.SetActive(false);
            hudPanel.SetActive(true);

            PlayerInputManager.instance.enabled = false;
            _currOceanType = OceanType.Raise;

            for (int i = 0; i < _playersFloatName.Count; i++)
                Destroy(_playersFloatName[i].gameObject);

            _playersFloatName.Clear();

            gmData.ChangeGameState("Game");
        }

        /// <summary>
        /// Continue match with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator ContinueMatchDelay()
        {
            if (PlayerNo > 1)
            {
                _currOceanType = OceanType.Reset;
                timerPanel.SetActive(true);

                startingRoundTimerText.text = "3";
                yield return new WaitForSeconds(1f);
                startingRoundTimerText.text = "2";
                yield return new WaitForSeconds(1f);
                startingRoundTimerText.text = "1";
                yield return new WaitForSeconds(1f);

                UpdatePlayerIndex();
                ChooseBombPlayer();

                timerPanel.SetActive(false);

                _currOceanType = OceanType.Raise;
                gmData.ChangeGameState("Game");
            }
            else
            {
                StartCoroutine(EndMatchDelay());

                _currOceanType = OceanType.Reset;
                gmData.ChangeGameState("End");
                //Debug.Log("Match Ended");
            }

        }

        /// <summary>
        /// Ends match with a Delay
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator EndMatchDelay()
        {
            PopupText($"Winner is {_playersController[0].name}");
            yield return new WaitForSeconds(3f);
            StartCoroutine(MenuDelay());
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
            // Sets up all the player controllers;
            _playersController.Add(plyBall);
            _playersCharController.Add(plyBall.GetComponent<CharacterController>());
            _playersCol.Add(plyBall.GetComponent<CapsuleCollider>());

            // Sets ui the Visual Datas;
            _playersController[PlayerNo].name = $"{playerVisData[PlayerNo].playerName}";
            _playersController[PlayerNo].GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", playerVisData[PlayerNo].playerColour);
            _playersController[PlayerNo].PlayerIndex = PlayerNo;

            // Spawns the Players
            _playersController[PlayerNo].transform.position = SetPlayerSpawns().position;
            _playersCharController[PlayerNo].enabled = true;

            // Sets Up Player floating Names;
            GameObject plyFloatName = Instantiate(playerFloatPrefab, _playersController[PlayerNo].transform.position, Quaternion.identity, plyBall.transform);
            PlayerFloatingName floatingName = plyFloatName.GetComponent<PlayerFloatingName>();
            _playersFloatName.Add(floatingName);
            floatingName.FloatingNameText.text = playerVisData[PlayerNo].playerName;
            floatingName.FollowPos = plyBall.gameObject.transform;
            floatingName.name = $"{playerVisData[PlayerNo].playerName}_Name_Canvas_World";

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

        void OnPlayerPassBombEventReceived(int index)
        {
            _currBombPlayerIndex = index;
            _playersController[index].PlayerBomber = true;
            _playersController[index].BombObj.SetActive(true);
            _playersCol[index].enabled = true;
        }
        #endregion
    }
}
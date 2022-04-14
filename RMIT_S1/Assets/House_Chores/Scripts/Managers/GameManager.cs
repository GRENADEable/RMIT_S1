using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace Khatim
{
    public class GameManager : MonoBehaviour
    {
        #region Serialized Variables

        #region Datas
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerData gmData = default;

        [SerializeField]
        [Tooltip("Do you want to disable Cursor?")]
        private bool isCursorDisabled = default;

        [SerializeField]
        [Tooltip("Array of Objective Scriptable Objects")]
        private ObjectiveData[] objData = default;
        #endregion

        #region UI

        #region Pause
        [Space, Header("Pause UI")]
        [SerializeField]
        [Tooltip("Pause UI GameObject")]
        private GameObject pausePanel;

        [SerializeField]
        [Tooltip("The Buttons on the Pause Menu")]
        private Button[] pauseButtons;
        #endregion

        #region Player HUD
        [Space, Header("Player HUD UI")]
        [SerializeField]
        [Tooltip("The Player's HUD UI GameObject")]
        private GameObject hudPanel;

        [SerializeField]
        [Tooltip("Fade panel Animation Component")]
        private Animator fadeBG = default;

        [SerializeField]
        [Tooltip("Objective Text Component")]
        private TextMeshProUGUI objText = default;
        #endregion

        #endregion

        #region Player

        #endregion

        #region Bread
        [SerializeField]
        [Tooltip("Bread GameObject")]
        private GameObject bread;

        [SerializeField]
        [Tooltip("Bread Spawn Positions")]
        private Transform[] brotPos;
        #endregion

        #endregion

        #region Private Variables
        private bool _isPaused = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            CoffeeObjective.OnShowObj2 += OnShowObj2EventReceived;
            CoffeeObjective.OnShowObj3 += OnShowObj3EventReceived;
        }

        void OnDisable()
        {
            CoffeeObjective.OnShowObj2 -= OnShowObj2EventReceived;
            CoffeeObjective.OnShowObj3 -= OnShowObj3EventReceived;
        }

        void OnDestroy()
        {
            CoffeeObjective.OnShowObj2 -= OnShowObj2EventReceived;
            CoffeeObjective.OnShowObj3 -= OnShowObj3EventReceived;
        }
        #endregion

        void Start()
        {
            gmData.ChangeGameState("Intro");
            StartCoroutine(StartDelay());
            ShowObjectiveNum(0);

            if (isCursorDisabled)
                gmData.DisableCursor();

            SetBrotPos();
        }

        void Update()
        {

        }
        #endregion

        #region My Functions

        #region Buttons
        /// <summary>
        /// Function tied with Resume_Button Button;
        /// Resumes the Game;
        /// </summary>
        public void OnClick_Resume()
        {
            _isPaused = !_isPaused;
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
        public void OnClick_Restart() => StartCoroutine(RestartGameDelay());

        /// <summary>
        /// Button tied with Menu_Button;
        /// Goes to the Menu with a delay;
        /// </summary>
        public void OnClick_Menu() => StartCoroutine(MenuDelay());

        /// <summary>
        /// Function tied with Quit_Button Buttons;
        /// Quits the game with a delay;
        /// </summary>
        public void OnClick_Quit() => StartCoroutine(QuitGameDelay());

        /// <summary>
        /// Function tied with Restart_Button, Menu_Button and Quit_Button Buttons;
        /// Disables the buttons so the Player can't interact with them when the panel is fading out;
        /// </summary>
        public void OnClick_DisableButtons()
        {
            for (int i = 0; i < pauseButtons.Length; i++)
                pauseButtons[i].interactable = false;
        }

        /// <summary>
        /// Function tied with Game_Start_Signal Signal;
        /// This enables the player controls;
        /// </summary>
        public void OnIntroEndStartGame() => StartCoroutine(StartDelay());
        #endregion

        #region Objectives
        /// <summary>
        /// Shows objective depending on the objective Number from the Scriptable Object;
        /// </summary>
        /// <param name="objNum"> Must have int index objective number; </param>
        void ShowObjectiveNum(int objNum) => objText.text = objData[objNum].objectiveMessage;

        /// <summary>
        /// Sets bread spawn Position when the Scene starts;
        /// </summary>
        void SetBrotPos()
        {
            int index = Random.Range(0, brotPos.Length);
            bread.transform.position = brotPos[index].position;
        }
        #endregion

        #endregion

        #region Coroutines
        /// <summary>
        /// Starts game with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator StartDelay()
        {
            hudPanel.SetActive(true);
            fadeBG.Play("Fade_In");
            yield return new WaitForSeconds(0.5f);
            gmData.ChangeGameState("Game");
        }

        /// <summary>
        /// Kills the player with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator KillPlayerDelay()
        {
            fadeBG.Play("Fade_Out");
            gmData.ChangeGameState("Dead");
            yield return new WaitForSeconds(0.5f);
            fadeBG.Play("Fade_In");
        }

        /// <summary>
        /// Restarts the game with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator RestartGameDelay()
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
        /// Quits with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator QuitGameDelay()
        {
            gmData.TogglePause(false);
            fadeBG.Play("Fade_Out");
            yield return new WaitForSeconds(0.5f);
            gmData.QuitGame();
        }
        #endregion

        #region Events

        #region Input Systems
        /// <summary>
        /// Function tied with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnPauseToggle(InputAction.CallbackContext context)
        {
            if (context.started && (gmData.currState != GameManagerData.GameState.Intro || gmData.currState != GameManagerData.GameState.End))
            {
                _isPaused = !_isPaused;

                if (_isPaused)
                {
                    gmData.EnableCursor();
                    gmData.ChangeGameState("Paused");
                    pausePanel.SetActive(true);
                    gmData.TogglePause(true);
                    hudPanel.SetActive(false);
                }
                else
                {
                    gmData.DisableCursor();
                    gmData.ChangeGameState("Game");
                    pausePanel.SetActive(false);
                    gmData.TogglePause(false);
                    hudPanel.SetActive(true);
                }
            }
        }
        #endregion

        #region UI
        /// <summary>
        /// Subbed to event from PasscodeHandler Script;
        /// Enables and disables Cursor and changes Game State;
        /// </summary>
        void OnCursorVisibleEventReceived(bool isShown)
        {
            if (isShown)
            {
                gmData.EnableCursor();
                gmData.ChangeGameState("Passcode");
            }
            else
            {
                gmData.DisableCursor();
                gmData.ChangeGameState("Game");
            }
        }
        #endregion

        #region Objectives UI
        /// <summary>
        /// Subbed to event from CoffeeObjective;
        /// Shows 2nd Objective;
        /// </summary>
        void OnShowObj2EventReceived() => ShowObjectiveNum(1);

        /// <summary>
        /// Subbed to event from CoffeeObjective;
        /// Shows 3rd Objective;
        /// </summary>
        void OnShowObj3EventReceived() => ShowObjectiveNum(2);
        #endregion

        #endregion
    }
}
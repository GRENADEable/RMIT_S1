using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Khatim_F2
{
    public class GameManagerLobby : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerDataMiniGame gmData = default;

        [SerializeField]
        [Tooltip("Fade panel Animation Component")]
        private Animator fadeBG = default;

        [Tooltip("Menu Button in an Array that will be used to disable them when clicking on other Buttons")]
        [SerializeField]
        private Button[] menuButtons;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            gmData.EnableCursor();
            gmData.ChangeGameState("Menu");
            fadeBG.Play("Fade_In");
        }
        #endregion

        #region My Functions

        #region Buttons
        /// <summary>
        /// Button tied with Start_Button;
        /// Starts the Game
        /// </summary>
        public void OnClick_StartGame() => StartCoroutine(StartGameDelay());

        /// <summary>
        /// Button tied with Quit_Button;
        /// Quits the Game
        /// </summary>
        public void OnClick_QuitGame() => StartCoroutine(QuitGameDelay());

        /// <summary>
        /// All the buttons added in the Array gets disabled;
        /// </summary>
        public void DisableButtons()
        {
            for (int i = 0; i < menuButtons.Length; i++)
                menuButtons[i].interactable = false;
        }
        #endregion

        #endregion

        #region Coroutines
        /// <summary>
        /// Starts the game with a Delay;
        /// </summary>
        /// <returns> Float Delay </returns>
        IEnumerator StartGameDelay()
        {
            fadeBG.Play("Fade_Out");
            yield return new WaitForSeconds(0.5f);
            gmData.ChangeLevel(1);
        }

        /// <summary>
        /// Quits the game with a Delay;
        /// </summary>
        /// <returns> Float Delay </returns>
        IEnumerator QuitGameDelay()
        {
            fadeBG.Play("Fade_Out");
            yield return new WaitForSeconds(0.5f);
            gmData.QuitGame();
        }
        #endregion
    }
}
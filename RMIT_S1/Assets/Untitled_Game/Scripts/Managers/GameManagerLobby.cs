using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        [SerializeField]
        [Tooltip("All the first button that the Event System will highlight")]
        private GameObject[] firstSelectedButtons = default;
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
        public void OnClick_StartGame(int index) => StartCoroutine(StartGameDelay(index));

        /// <summary>
        /// Button tied with Quit_Button;
        /// Quits the Game
        /// </summary>
        public void OnClick_QuitGame() => StartCoroutine(QuitGameDelay());

        /// <summary>
        /// All the buttons added in the Array gets disabled;
        /// </summary>
        public void OnClick_DisableButtons()
        {
            for (int i = 0; i < menuButtons.Length; i++)
                menuButtons[i].interactable = false;
        }

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
        #endregion

        #endregion

        #region Coroutines
        /// <summary>
        /// Starts the game with a Delay;
        /// </summary>
        /// <returns> Float Delay </returns>
        IEnumerator StartGameDelay(int sceneIndex)
        {
            fadeBG.Play("Fade_Out");
            yield return new WaitForSeconds(0.5f);
            gmData.ChangeLevel(sceneIndex);
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
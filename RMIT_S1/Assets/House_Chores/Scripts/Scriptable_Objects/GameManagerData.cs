using UnityEngine;

namespace Khatim
{
    [CreateAssetMenu(fileName = "GameManager_Data", menuName = "Managers/GameManagerData")]
    public class GameManagerData : ScriptableObject
    {
        #region Public Variables
        [Space, Header("Enums")]
        public GameState currState = GameState.Game;
        public enum GameState { Menu, Intro, Game, Paused, Outro, End };
        #endregion

        #region Private Variables

        #endregion

        #region My Functions

        #region Cursor
        public void EnableCursor()
        {
            LockCursor(false);
            VisibleCursor(true);
        }

        public void DisableCursor()
        {
            LockCursor(true);
            VisibleCursor(false);
        }

        /// <summary>
        /// Locks the user's cusor;
        /// </summary>
        /// <param name="isLocked"> If true, lock the cursor in place, if false, free the cursor; </param>
        void LockCursor(bool isLocked)
        {
            if (isLocked)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// User's cursor visibility;
        /// </summary>
        /// <param name="isVisible"> If true, lock show cursor, if false, hide cursor; </param>
        void VisibleCursor(bool isVisible)
        {
            if (isVisible)
                Cursor.visible = true;
            else
                Cursor.visible = false;
        }
        #endregion

        /// <summary>
        /// Pauses game using Time.timeScale;
        /// </summary>
        /// <param name="isPaused"> If true, pause game, if false, un-pause game; </param>
        public void TogglePause(bool isPaused)
        {
            if (isPaused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }

        #region Game States
        /// <summary>
        /// Changes the state the game is running on using enums;
        /// </summary>
        /// <param name="state"> Uses string to check which state to change to. The string has to be exact or else it won't work; </param>
        public void ChangeGameState(string state)
        {
            if (state.Contains("Menu"))
                currState = GameState.Menu;

            if (state.Contains("Intro"))
                currState = GameState.Intro;

            if (state.Contains("Game"))
                currState = GameState.Game;

            if (state.Contains("Paused"))
                currState = GameState.Paused;

            if (state.Contains("Outro"))
                currState = GameState.Outro;

            if (state.Contains("End"))
                currState = GameState.End;
        }

        /// <summary>
        /// Changes Level using the int Values;
        /// </summary>
        /// <param name="level"> Int variable for the Scenes added in Build Windows; </param>
        public void ChangeLevel(int level) => Application.LoadLevel(level);

        /// <summary>
        /// Closes the Game;
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("Game Closed");
        }

        #endregion

        #endregion
    }
}
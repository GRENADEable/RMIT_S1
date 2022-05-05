using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim_F2
{
    public class GameManagerPlatformDuel : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerDataMiniGame gmData = default;

        [SerializeField]
        [Tooltip("Do you want to disable Cursor?")]
        private bool isCursorDisabled = default;

        [Space, Header("UI")]
        [SerializeField]
        [Tooltip("Fade panel Animation Component")]
        private Animator fadeBG = default;

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
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {

        }

        void OnDisable()
        {

        }

        void OnDestroy()
        {

        }
        #endregion

        void Start()
        {
            StartCoroutine(StartDelay());
        }

        void Update()
        {
            // Testing Control Switch;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _isSwitchingControls = !_isSwitchingControls;
                OnControlsSwitched?.Invoke(_isSwitchingControls);
                OnControlsJump?.Invoke(_isSwitchingControls);
            }
        }
        #endregion

        #region My Functions

        #endregion

        #region Coroutines
        /// <summary>
        /// Starts game with a Delay;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator StartDelay()
        {
            fadeBG.Play("Fade_In");
            yield return new WaitForSeconds(0.5f);
            gmData.ChangeGameState("Game");
        }
        #endregion

        #region Events

        #endregion
    }
}
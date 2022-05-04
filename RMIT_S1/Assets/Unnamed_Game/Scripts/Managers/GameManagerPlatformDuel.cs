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
        #endregion

        #region Private Variables

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
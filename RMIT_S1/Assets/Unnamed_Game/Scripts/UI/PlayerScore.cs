using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Khatim_F2
{
    public class PlayerScore : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Player Name Text")]
        private TextMeshProUGUI playerName = default;

        [SerializeField]
        [Tooltip("Player Score Text")]
        private TextMeshProUGUI playerPoints = default;
        #endregion

        #region Private Variables
        public string PlayerName { get => _playerName; set => _playerName = value; }
        [SerializeField] private string _playerName = default;

        public int PlayerPointIndex { get => _currPointIndex; set => _currPointIndex = value; }
        [SerializeField] private int _currPointIndex = default;
        public int PlayerPoints { get => _currPoints; set => _currPoints = value; }
        [SerializeField] private int _currPoints = default;
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
            IntialiseUI();
        }

        void Update()
        {

        }
        #endregion

        #region My Functions
        /// <summary>
        /// Setting up intial values of the UI;
        /// </summary>
        void IntialiseUI()
        {
            PlayerPoints = 0;
            playerPoints.text = $"{PlayerPoints}";
            playerName.text = $"{PlayerName}";
        }

        public void UpdateScore(int score)
        {
            PlayerPoints += score;
            playerPoints.text = $"{PlayerPoints}";
        }
        #endregion
    }
}
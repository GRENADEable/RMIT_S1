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
        //[SerializeField]
        //[Tooltip("")]
        #endregion

        #region Private Variables
        private int _currScore = default;
        private Image _playerImg = default;
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
            _playerImg = GetComponentInChildren<Image>();
        }

        void Update()
        {

        }
        #endregion

        #region My Functions

        #endregion

        #region Coroutines

        #endregion

        #region Events

        #endregion
    }
}
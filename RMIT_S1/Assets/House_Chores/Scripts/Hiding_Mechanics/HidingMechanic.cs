using UnityEngine;

namespace Khatim
{
    public class HidingMechanic : MonoBehaviour
    {
        #region Serialized Variables
        public delegate void SendEventsBool(bool isHiding);
        /// <summary>
        /// Event sent fgrom HidingMechanic to EnemyFSM;
        /// Doesn't chase the player when the player is hidden;
        /// </summary>
        public static event SendEventsBool OnPlayerHide;
        #endregion

        #region Private Variables
        private SwingController _swController;
        private Collider _playerCol = default;
        private bool _isHiding = default;
        private bool _isHidingEventSent = false;
        private bool _isNotHidingEventSent = false;
        #endregion

        #region Unity Callbacks
        void Start() => _swController = GetComponent<SwingController>();

        void Update() => HideCheck();

        #region Triggers
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                _playerCol = other;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerCol = null;
                _isHidingEventSent = false;
                _isNotHidingEventSent = false;
            }
        }
        #endregion

        #endregion

        #region My Functions
        void HideCheck()
        {
            if (_playerCol != null && _swController != null)
            {
                if (!_swController.IsDoorOpened())
                {
                    _isHiding = true;

                    if (!_isHidingEventSent)
                    {
                        _isHidingEventSent = true;
                        _isNotHidingEventSent = false;
                        OnPlayerHide?.Invoke(true);
                        //Debug.Log("Player Hiding");
                    }
                }
                else
                {
                    _isHiding = false;

                    if (!_isNotHidingEventSent)
                    {
                        _isNotHidingEventSent = true;
                        _isHidingEventSent = false;
                        OnPlayerHide?.Invoke(false);
                        //Debug.Log("Player Not Hiding");
                    }
                }
            }
        }
        #endregion
    }
}
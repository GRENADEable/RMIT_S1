using UnityEngine;

namespace Khatim
{
    public class TVRemote : MonoBehaviour
    {
        #region Public Variables
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from TVRemote to ScreenImageManager Script;
        /// Changes the images on Interaction;
        /// </summary>
        public static event SendEvents OnChannelChange;

        /// <summary>
        /// Event sent from TVRemote to GameManager Script;
        /// Shows 6th Objective;
        /// </summary>
        public static event SendEvents OnShowObj6;
        #endregion

        #region Private Variables
        private bool _isPickable = default;
        private bool _canUpdateObj = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            PropHolder.OnRemotePicked += OnRemotePickedEventReceived;

            GameManager.OnRemoteObjDisable += OnRemoteObjDisableEventReceived;
        }

        void OnDisable()
        {
            PropHolder.OnRemotePicked -= OnRemotePickedEventReceived;

            GameManager.OnRemoteObjDisable -= OnRemoteObjDisableEventReceived;
        }

        void OnDestroy()
        {
            PropHolder.OnRemotePicked -= OnRemotePickedEventReceived;

            GameManager.OnRemoteObjDisable -= OnRemoteObjDisableEventReceived;
        }
        #endregion

        void Start() => _canUpdateObj = true;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && _isPickable)
            {
                OnChannelChange?.Invoke();

                if (_canUpdateObj)
                    OnShowObj6?.Invoke();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from PropHolder;
        /// Lets Player switch channel;
        /// </summary>
        /// <param name="isPicked"> If true, can switch channels, if false, can't switch channels; </param>
        void OnRemotePickedEventReceived(bool isPicked)
        {
            if (isPicked)
                _isPickable = true;
            else
                _isPickable = false;
        }

        /// <summary>
        /// Subbed to event from GameManager;
        /// Disables objective updates;
        /// </summary>
        void OnRemoteObjDisableEventReceived() => _canUpdateObj = false;
        #endregion
    }
}
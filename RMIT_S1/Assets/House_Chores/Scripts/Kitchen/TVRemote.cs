using UnityEngine;

namespace Khatim
{
    public class TVRemote : MonoBehaviour
    {
        #region Public Variables
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from TVRemote to ScreenImageManager;
        /// Changes the images on Interaction;
        /// </summary>
        public static event SendEvents OnChannelChange;
        #endregion

        #region Private Variables
        private bool _isPickable = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => PropHolder.OnRemotePicked += OnRemotePickedEventReceived;

        void OnDisable() => PropHolder.OnRemotePicked -= OnRemotePickedEventReceived;

        void OnDestroy() => PropHolder.OnRemotePicked -= OnRemotePickedEventReceived;
        #endregion

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && _isPickable)
                OnChannelChange?.Invoke();
        }
        #endregion

        #region Events
        void OnRemotePickedEventReceived(bool isPicked)
        {
            if (isPicked)
                _isPickable = true;
            else
                _isPickable = false;
        }
        #endregion
    }
}
using UnityEngine;

namespace Khatim
{
    public class TVRemote : MonoBehaviour
    {
        #region Serialized Variables
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from TVRemote to ScreenImageManager;
        /// Changes the images on Interaction;
        /// </summary>
        public static event SendEvents OnChannelChange;
        #endregion

        #region Unity Callbacks
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
                OnChannelChange?.Invoke();
        }
        #endregion
    }
}
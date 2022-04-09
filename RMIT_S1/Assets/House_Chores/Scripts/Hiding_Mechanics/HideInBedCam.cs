using UnityEngine;

namespace MadInc
{
    public class HideInBedCam : MonoBehaviour
    {
        #region Serialized Variables
        public delegate void SendEventsBool(bool isHiding);
        /// <summary>
        /// Event sent fgrom HideInBedCam to EnemyFSM;
        /// Doesn't chase the player when the player is hidden;
        /// </summary>
        public static event SendEventsBool OnPlayerHide;
        #endregion

        #region Unity Callbacks
        void OnEnable()
        {
            OnPlayerHide?.Invoke(true);
            Debug.Log("Player Hiding Under Bed");
        }

        void OnDisable()
        {
            OnPlayerHide?.Invoke(false);
            Debug.Log("Player Not Hiding Under Bed");
        }

        void OnDestroy()
        {
            OnPlayerHide?.Invoke(false);
            Debug.Log("Player Not Hiding Under Bed");
        }
        #endregion
    }
}
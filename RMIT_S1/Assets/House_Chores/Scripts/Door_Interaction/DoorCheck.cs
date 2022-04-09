using UnityEngine;

namespace Khatim
{
    public class DoorCheck : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Player tag to compare with?")]
        private string playerTag = "Player";
        #endregion

        #region Private Variables
        private SwingController _door = default;
        #endregion

        #region Unity Callbacks
        void Start() => _door = GetComponentInParent<SwingController>();

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
                _door._isOnHold = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
                _door._isOnHold = false;
        }
        #endregion
    }
}
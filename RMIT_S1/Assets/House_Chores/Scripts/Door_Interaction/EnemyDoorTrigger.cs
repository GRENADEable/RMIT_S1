using UnityEngine;

namespace MadInc
{
    public class EnemyDoorTrigger : MonoBehaviour
    {
        #region Private Variables
        private SwingController _swController = default;
        #endregion

        #region Unity Callbacks
        void Start() => _swController = GetComponent<SwingController>();

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy") && _swController != null)
                _swController.OpenDoorEnemy();
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy") && _swController != null)
                _swController.CloseDoorEnemy();
        }
        #endregion
    }
}
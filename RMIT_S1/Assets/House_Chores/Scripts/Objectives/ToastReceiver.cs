using UnityEngine;

namespace Khatim
{
    public class ToastReceiver : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Toaster Trigger String")]
        private string toasterString = default;

        public delegate void SendEventsGameObject(GameObject obj);
        /// <summary>
        /// Event sent from ToastReceiver to ToastObjective;
        /// Increments the bread counter by 1;
        /// </summary>
        public static event SendEventsGameObject OnBrotAdded;
        #endregion

        #region Unity Callbacks
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(toasterString))
            {
                other.enabled = false;
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                gameObject.GetComponent<Collider>().isTrigger = true;
                transform.position = other.transform.position;
                transform.rotation = other.transform.rotation;
                transform.parent = other.transform.parent;
                OnBrotAdded?.Invoke(gameObject);
            }
        }
        #endregion
    }
}
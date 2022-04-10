using UnityEngine;

namespace Khatim
{
    public class PickableItems : MonoBehaviour
    {
        #region Serialized Variables

        #region Examine Variables
        [Space, Header("Examine Variables")]
        private bool _isInteractable;
        public bool IsInteractable { get { return _isInteractable; } set { _isInteractable = value; } }

        [Tooltip("Vector3 position offset of the Examine GameObject")]
        public Vector3 holdPropPosOffset = Vector3.zero;

        [Tooltip("Vector3 rotation offset of the Examine GameObject")]
        public Vector3 holdPropRotOffset = Vector3.zero;

        [Tooltip("Vector3 scale of the Examine GameObject")]
        public Vector3 holdPropScale = Vector3.one;
        #endregion

        #region Examine Description
        //[Space, Header("Examine Description")]
        //[TextArea(5, 5)]
        //[Tooltip("Item information")]
        //public string itemDescription = "New Description";

        //[Tooltip("Item information Text Color")]
        //public Color itemDescriptionTextColor = Color.black;

        //[Tooltip("Item information Text Size")]
        //public float itemDescriptionTextSize = 50f;
        #endregion

        #endregion

        #region Private Variables
        [Header("Examine Variables")]
        private Rigidbody _rb = default;
        private Vector3 _intialObjScale = default;
        private Quaternion _intialRot = default;
        #endregion

        #region Unity Callbacks

        void Start() => Intialise();
        #endregion

        #region My Functions
        /// <summary>
        /// Gets references of the item this script is attached to;
        /// Stores this item's position and rotation in a scriptable object;
        /// </summary>
        void Intialise()
        {
            _rb = GetComponent<Rigidbody>();
            _intialObjScale = transform.localScale;
            _intialRot = transform.rotation;
        }

        public void OnInteractable() => IsInteractable = true;

        public void StartInteraction()
        {
            _rb.isKinematic = true;
            gameObject.layer = LayerMask.NameToLayer("PropHoldLayer");
        }

        public void EndInteraction()
        {
            transform.localScale = _intialObjScale;
            transform.rotation = _intialRot;
            _rb.isKinematic = false;
        }
        #endregion
    }
}
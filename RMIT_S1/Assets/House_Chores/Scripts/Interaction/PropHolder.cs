using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim
{
    public class PropHolder : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerData gmData = default;

        #region Prop Variables
        [Space, Header("Prop Variables")]
        [Tooltip("Debug Ray for Editor")]
        [SerializeField]
        private bool isDebugging = default;

        [Tooltip("Transform Pos where the Prop will be Held")]
        [SerializeField]
        private Transform propHoldPos;

        [Tooltip("Transform Pos where the Prop will be Dropped")]
        [SerializeField]
        private Transform propDropPos;
        #endregion

        #region Interact Raycast
        [Space, Header("Interact Raycast")]
        [SerializeField]
        [Tooltip("Prop Layer for Ray")]
        private LayerMask propLayer;

        [SerializeField]
        [Tooltip("Ray distance for interaction")]
        private float rayDistance = default;

        //[SerializeField]
        //[Tooltip("Prop Layer for Ray")]
        //private LayerMask obstacleLayer;
        #endregion

        #region Events

        #region Int Events
        public delegate void SendEventsInt(int value);
        /// <summary>
        /// Event sent from PropHolder Script to Keyholder Script;
        /// Sends Int Value to be added to the list
        /// </summary>
        public static event SendEventsInt OnKeyHeld;

        /// <summary>
        /// Event sent from PropHolder Script to Keyholder Script;
        /// Sends Int Value to be removed to the list
        /// </summary>
        public static event SendEventsInt OnKeyDrop;
        #endregion

        #region Bool Events
        public delegate void SendEventsBool(bool isPicked);
        /// <summary>
        /// Event sent from PropHolder to TVRemote;
        /// Makes the remove Interactable;
        /// </summary>
        public static event SendEventsBool OnRemotePicked;
        #endregion

        #endregion

        #endregion

        #region Private Variables
        [Header("Interact Raycast")]
        private Camera _cam = default;
        private bool _isHitting = default;
        //[SerializeField] private bool _isBlocking = default;
        private bool _isInteractHoldButtonPressed = default;
        private bool _isInteractDropButtonPressed = default;
        private bool _isHoldingItem = default;
        private Ray _ray = default;
        private RaycastHit _hit = default;

        [Header("Prop Variables")]
        private bool _canPickItems = true;
        private PickableItems _tempPickItem = default;
        private GameObject _tempObjReference = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            DoorTrigger.OnKeyUsed += OnKeyUsedEventReceived;
        }

        void OnDisable()
        {
            DoorTrigger.OnKeyUsed -= OnKeyUsedEventReceived;
        }

        void OnDestroy()
        {
            DoorTrigger.OnKeyUsed -= OnKeyUsedEventReceived;
        }
        #endregion

        void Start()
        {
            _cam = Camera.main;
            _canPickItems = true;
        }

        void Update()
        {
            if (gmData.currState == GameManagerData.GameState.Game)
            {
                _ray = new Ray(_cam.transform.position, _cam.transform.forward);
                RaycastCheckProp();
            }
        }
        #endregion

        #region My Functions

        #region Raycast Checks
        /// <summary>
        /// Check for Key Pickup;
        /// </summary>
        void RaycastCheckProp()
        {
            _isHitting = Physics.Raycast(_ray, out _hit, rayDistance, propLayer);
            //_isBlocking = Physics.Raycast(_ray, rayDistance, obstacleLayer);

            if (isDebugging)
                Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, _isHitting ? Color.red : Color.white);

            if (_canPickItems)
            {
                if (_isHitting /*&& !_isBlocking*/)
                {
                    // Hold Item or Pickup;
                    if (_isInteractHoldButtonPressed)
                    {
                        if (_hit.collider.GetComponent<PickableItems>() != null)
                        {
                            _isInteractHoldButtonPressed = false;
                            _tempPickItem = _hit.collider.GetComponent<PickableItems>();
                            _tempPickItem.StartInteraction();

                            HoldItem(_hit.collider.gameObject);

                            if (_tempPickItem.GetComponent<TVRemote>() != null)
                                OnRemotePicked?.Invoke(true);

                            _isHoldingItem = true;
                            _canPickItems = false;
                        }
                    }
                }
            }

            if (!_isHitting)
            {
                // Drop Item;
                if (_isHoldingItem && _isInteractDropButtonPressed)
                {
                    DropItem();
                    _tempPickItem.EndInteraction();
                    OnRemotePicked?.Invoke(false);
                    _isHoldingItem = false;
                    _canPickItems = true;
                }
            }
        }
        #endregion

        #region Prop Interactions
        /// <summary>
        /// Moves Prop to position, makes Rg Kinematic, stores temp variables;
        /// </summary>
        /// <param name="prop"> GameObject parameter from hitInfo; </param>
        void HoldItem(GameObject pickProp)
        {
            _tempObjReference = pickProp;

            _tempObjReference.transform.parent = propHoldPos;
            _tempObjReference.transform.SetPositionAndRotation(propHoldPos.position + _tempPickItem.holdPropPosOffset, propHoldPos.rotation);
            _tempObjReference.transform.localScale = _tempPickItem.holdPropScale;
            _tempObjReference.transform.Rotate(_tempPickItem.holdPropRotOffset);

            if (_tempObjReference.GetComponent<KeyItem>() != null)
                OnKeyHeld?.Invoke(_tempObjReference.GetComponent<KeyItem>().GetKeyTypeInt());
        }

        /// <summary>
        /// Drops Item infront of Player, disables Rg Kinematic, clears temp Variables;
        /// </summary>
        void DropItem()
        {
            _tempObjReference.layer = LayerMask.NameToLayer("PropLayer");
            _tempObjReference.transform.position = propDropPos.position;
            _tempObjReference.transform.parent = null;

            if (_tempObjReference.GetComponent<KeyItem>() != null)
                OnKeyDrop?.Invoke(_tempObjReference.GetComponent<KeyItem>().GetKeyTypeInt());

            ResetValues();
        }

        /// <summary>
        /// Destroys the item and removes Reference values;
        /// </summary>
        void DestroyItem()
        {
            Destroy(_tempObjReference.gameObject);
            ResetValues();
            _isHoldingItem = false;
            _canPickItems = true;
        }

        /// <summary>
        /// Resets the Temp values;
        /// </summary>
        void ResetValues() => _tempObjReference = null;
        #endregion

        #endregion

        #region Events

        #region Input Systems
        /// <summary>
        /// Event subbed with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnInteractHold(InputAction.CallbackContext context) => _isInteractHoldButtonPressed = context.ReadValueAsButton();

        /// <summary>
        /// Event subbed  with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnInteractDrop(InputAction.CallbackContext context) => _isInteractDropButtonPressed = context.ReadValueAsButton();
        #endregion

        /// <summary>
        /// Subbed to Event from DoorTrigger;
        /// Destroys the key that the player is currently holding;
        /// </summary>
        void OnKeyUsedEventReceived()
        {
            DestroyItem();
            Debug.Log("Destroyed Key");
        }
        #endregion
    }
}
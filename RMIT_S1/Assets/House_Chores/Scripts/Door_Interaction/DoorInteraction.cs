using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim
{
    public class DoorInteraction : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerData gmData = default;

        [Space, Header("Key Raycast")]
        [Tooltip("Debug Ray for Editor")]
        [SerializeField]
        private bool isDebugging = default;

        [SerializeField]
        [Tooltip("Which layer(s) is the door?")]
        private LayerMask doorLayer = default;

        [SerializeField]
        [Tooltip("Raycast distance from the player camera")]
        private float rayDistance = 2f;
        #endregion

        #region Private Variables
        private List<int> _keyItemsInt = new List<int>();
        private Camera _cam = default;
        private bool _isInteractingDoor = default;
        private bool _isInteractButtonPressed = default;
        private Ray _ray = default;
        private RaycastHit _hit = default;
        private PropHighlight _highlightProp;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            PropHolder.OnKeyHeld += OnKeyHeldEventReceived;
            PropHolder.OnKeyDrop += OnKeyDropEventReceived;
        }

        void OnDisable()
        {
            PropHolder.OnKeyHeld -= OnKeyHeldEventReceived;
            PropHolder.OnKeyDrop -= OnKeyDropEventReceived;
        }

        void OnDestroy()
        {
            PropHolder.OnKeyHeld -= OnKeyHeldEventReceived;
            PropHolder.OnKeyDrop -= OnKeyDropEventReceived;
        }
        #endregion

        #region Initialisation and Loops
        void Start() => _cam = Camera.main;

        void Update()
        {
            if (gmData.currState == GameManagerData.GameState.Game)
            {
                _ray = new Ray(_cam.transform.position, _cam.transform.forward);
                RaycastCheckDoor();
            }
        }
        #endregion

        #endregion

        #region My Functions

        #region Keys
        /// <summary>
        /// Adds key iD to the list;
        /// </summary>
        /// <param name="keyInt"> Key iD parameter; </param>
        public void AddKeyInt(int keyInt) => _keyItemsInt.Add(keyInt);

        /// <summary>
        /// Removes key iD from the list;
        /// </summary>
        /// <param name="keyInt"> Key iD parameter; </param>
        public void RemoveKeyInt(int keyInt) => _keyItemsInt.Remove(keyInt);

        /// <summary>
        /// Checks key iD from the list;
        /// </summary>
        /// <param name="keyInt"> Key iD parameter; </param>
        public bool ContainKeyInt(int keyInt) => _keyItemsInt.Contains(keyInt);
        #endregion

        #region Raycast Checks
        /// <summary>
        /// Check for Door;
        /// </summary>
        void RaycastCheckDoor()
        {
            _isInteractingDoor = Physics.Raycast(_ray, out _hit, rayDistance, doorLayer);

            if (isDebugging)
                Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, _isInteractingDoor ? Color.red : Color.white);

            if (_isInteractingDoor)
            {
                if (_isInteractButtonPressed)
                {
                    if (_hit.collider.GetComponentInParent<DoorTrigger>() != null)
                        _hit.collider.GetComponentInParent<DoorTrigger>().InteractDoor(gameObject);

                    if (_hit.collider.GetComponentInParent<PropTriggerEvent>() != null)
                        _hit.collider.GetComponentInParent<PropTriggerEvent>().InteractPropRaycast();

                    // Temp fix for interaction with new input Systems;
                    _isInteractButtonPressed = false;
                }

                if (_hit.collider.GetComponentInParent<PropHighlight>() != null)
                {
                    _highlightProp = _hit.collider.GetComponentInParent<PropHighlight>();
                    _highlightProp.HighLightObject(true);
                }
                else
                    DisableHightlight();

            }
            else
                DisableHightlight();
        }
        #endregion

        void DisableHightlight()
        {
            if (_highlightProp != null)
            {
                _highlightProp.HighLightObject(false);
                _highlightProp = null;
            }
        }

        #endregion

        #region Events

        #region Input Systems
        /// <summary>
        /// Function tied with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnInteractKey(InputAction.CallbackContext context) => _isInteractButtonPressed = context.ReadValueAsButton();
        #endregion

        #region Key System
        /// <summary>
        /// Subbed to Event from PropHolder;
        /// Adds key to the list;
        /// </summary>
        /// <param name="value"> Key Value currently; </param>
        void OnKeyHeldEventReceived(int value) => AddKeyInt(value);

        /// <summary>
        /// Subbed to Event from PropHolder;
        /// Removes key from the list;
        /// </summary>
        /// <param name="value"> Key Value currently; </param>
        void OnKeyDropEventReceived(int value) => RemoveKeyInt(value);
        #endregion

        #endregion
    }
}
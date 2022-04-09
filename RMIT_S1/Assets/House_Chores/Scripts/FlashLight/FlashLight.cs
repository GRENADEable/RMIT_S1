using UnityEngine;
using UnityEngine.InputSystem;

namespace MadInc
{
    public class FlashLight : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerData gmData = default;

        #region Interact Raycast
        [Space, Header("Battery Interact Raycast")]
        [Tooltip("Debug Ray for Editor")]
        [SerializeField]
        private bool isDebugging = default;

        [SerializeField]
        [Tooltip("Ray distance for interaction")]
        private float rayDistance = default;

        [SerializeField]
        [Tooltip("Ray Layer for Battery")]
        private LayerMask battLayer;
        #endregion

        #region Flashlight
        [Space, Header("Flashlight")]
        [SerializeField]
        [Tooltip("Flashlight toggle, true to make the falshlight interact with the key, false to not make it interactable with the key")]
        private bool isToggleable = true;

        [SerializeField]
        [Tooltip("Flashlight root GameObject. Reference from the scene itself")]
        private GameObject flashLightSceneObj = default;

        [SerializeField]
        [Tooltip("Flashlight movment speed, higher the number, the faster it is")]
        private float followSpeed = 10f;
        #endregion

        #endregion

        #region Private Variables
        [Header("Light Follow")]
        private Vector3 _offset = default;
        private GameObject _objFollow = default;

        [Header("Battery Raycast")]
        private Ray _ray = default;
        private RaycastHit _hit = default;
        private Camera _cam = default;
        private bool _isInteractButtonPressed = default;
        private bool _isHitting = default;

        [Header("Battery Variables")]
        private bool _isEnabled = false;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => HideInBed.OnPlayerHidingUnderBed += OnPlayerHidingUnderBedEventReceived;

        void OnDisable() => HideInBed.OnPlayerHidingUnderBed -= OnPlayerHidingUnderBedEventReceived;

        void OnDestroy() => HideInBed.OnPlayerHidingUnderBed -= OnPlayerHidingUnderBedEventReceived;
        #endregion

        void Start()
        {
            _cam = Camera.main;
            _objFollow = _cam.gameObject;
            _offset = flashLightSceneObj.transform.position - _objFollow.transform.position;
        }

        void Update()
        {
            if (gmData.currState == GameManagerData.GameState.Game)
            {
                // Flashlight move around;
                if (flashLightSceneObj && isToggleable)
                    OffsetFlashLight();

                // Checks the batteries in the level with Raycast;
                RaycastBattCheck();
            }
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Battery check
        /// </summary>
        void RaycastBattCheck()
        {
            _ray = new Ray(_cam.transform.position, _cam.transform.forward);
            _isHitting = Physics.Raycast(_ray, out _hit, rayDistance, battLayer);

            if (isDebugging)
                Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, _isHitting ? Color.red : Color.white);

            if (_isHitting)
            {
                if (_isInteractButtonPressed)
                {
                    _isInteractButtonPressed = false;
                    Destroy(_hit.collider.gameObject);
                    //Debug.Log("Picking Battery");
                }
            }
        }

        #region Flashlight
        /// <summary>
        /// Smooth lerp the flashlight GameObject;
        /// </summary>
        void OffsetFlashLight()
        {
            flashLightSceneObj.transform.position = _objFollow.transform.position + _offset;
            flashLightSceneObj.transform.rotation = Quaternion.Slerp(flashLightSceneObj.transform.rotation, _objFollow.transform.rotation, followSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Enables and disables flahlight with Input Key;
        /// </summary>
        void ToggleFlashLight()
        {
            flashLightSceneObj.SetActive(!flashLightSceneObj.activeSelf);
            _isEnabled = !_isEnabled;
        }
        #endregion

        #endregion

        #region Events

        #region Input Systems
        /// <summary>
        /// Event subbed with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnLightToggle(InputAction.CallbackContext context)
        {
            if (context.started && gmData.currState == GameManagerData.GameState.Game)
                ToggleFlashLight();
        }
        #endregion

        #region Flashlight
        /// <summary>
        /// Subbed to event from HideInBed Script;
        /// Turns off the flashlight when under the bed;
        /// </summary>
        void OnPlayerHidingUnderBedEventReceived()
        {
            flashLightSceneObj.SetActive(false);
            _isEnabled = false;
        }
        #endregion

        #endregion
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim
{
    public class FPSController : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Datas")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerData gmData = default;

        #region Player Movement
        [Space, Header("Player Variables")]
        [SerializeField]
        [Tooltip("Walk speed of the player")]
        private float playerWalkSpeed = 3f;

        [SerializeField]
        [Tooltip("Run speed of the player")]
        private float playerRunSpeed = 5f;

        [SerializeField]
        [Tooltip("Gravity of the player when falling")]
        private float gravity = -19.62f;
        #endregion

        #region Player Crouch
        [Space, Header("Crouch Variables")]
        [SerializeField]
        [Tooltip("Layer Maks for Roof when Crouching")]
        private LayerMask roofMask = default;

        [SerializeField]
        [Tooltip("Ray Distance of the crouch")]
        private float rayRoofDistance = 1f;

        [SerializeField]
        [Tooltip("Crouch speed of the player")]
        private float crouchWalkSpeed = 1f;

        [SerializeField]
        [Tooltip("How much the CharacterController Collider shrinks when crouched")]
        private float crouchColShrinkValue = 0.7f; //Initial Value is 0.7f

        [SerializeField]
        [Tooltip("Where is the center of the CharacterController Collider")]
        private float crouchColCenterValue = 0.5f; //Initial Value is 0.5f

        [SerializeField]
        [Tooltip("Lerp Speed for Crouching")]
        private float crouchLerp = 5f;
        #endregion

        #region Player Grounding
        [Space, Header("Ground Check")]
        [SerializeField]
        [Tooltip("Transform Component for checking the ground")]
        private Transform groundCheck = default;

        [SerializeField]
        [Tooltip("Spherecast radius for the ground")]
        private float groundDistance = 0.4f;

        [SerializeField]
        [Tooltip("Which layer(s) is used for the ground")]
        private LayerMask groundMask = default;
        #endregion

        #endregion

        #region Private Variables

        #region Player Movement
        [Header("Player Variables")]
        private Vector3 _moveDirection = default;
        private Vector3 _vel = default;
        private float _currSpeed = default;
        private bool _isRunning = default;
        private CharacterController _charControl = default;
        #endregion

        #region Player Crouch
        [Header("Crouch Variables")]
        private bool _isCrouchingButtonPressed = default;
        private float _playerHeight = default;
        private float _playerCenter = default;
        private bool _isHittingRoof = default;
        private bool IsCrouching { get { return _isCrouchingButtonPressed || _isHittingRoof; } }
        #endregion

        #region Player Grounding
        [Header("Ground Check")]
        private bool _isGrounded = default;
        #endregion

        #endregion

        #region Unity Callbacks
        void Start()
        {
            _charControl = GetComponent<CharacterController>();

            _currSpeed = playerWalkSpeed;
            _playerHeight = _charControl.height;
            _playerCenter = _charControl.center.y;
        }

        void Update()
        {
            if (gmData.currState == GameManagerData.GameState.Game || gmData.currState == GameManagerData.GameState.Outro)
            {
                GroundCheck();
                PlayerCurrStance();
                PlayerMovement();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Death_Box"))
                Application.LoadLevel(Application.loadedLevel);
        }

        #endregion

        #region My Functions

        #region Player Checks
        /// <summary>
        /// Ground check for gavity and jumping;
        /// </summary>
        void GroundCheck()
        {
            if (!IsCrouching)
                _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (_isGrounded && _vel.y < 0)
                _vel.y = -2f;

            _vel.y += gravity * Time.deltaTime;
            _charControl.Move(_vel * Time.deltaTime);
        }

        /// <summary>
        /// Crouch check so the player doesn't get stuck when they stop crouching;
        /// </summary>
        void CheckCrouch()
        {
            Ray ray = new Ray(transform.position + _charControl.center - Vector3.up * _charControl.height * 0.5f, Vector3.up);

            _isHittingRoof = Physics.Raycast(ray.origin, ray.direction.normalized, out RaycastHit hit, rayRoofDistance, roofMask);
            Debug.DrawRay(ray.origin, ray.direction.normalized * rayRoofDistance, _isHittingRoof ? Color.green : Color.red);
        }
        #endregion

        #region Player Movement
        /// <summary>
        /// This is where the player movement takes place;
        /// </summary>
        void PlayerMovement() => _charControl.Move((transform.forward * _moveDirection.y + transform.right * _moveDirection.x).normalized
            * _moveDirection.sqrMagnitude * _currSpeed * Time.deltaTime);

        /// <summary>
        /// Checks stance if the player is Running or Crouching;
        /// </summary>
        void PlayerCurrStance()
        {
            float localHeight = _playerHeight;
            float localCenter = _playerCenter;

            if (IsCrouching) // Crouch
            {
                localHeight = _playerHeight * crouchColShrinkValue;
                localCenter = _playerCenter / crouchColCenterValue;
                _currSpeed = crouchWalkSpeed;
                //Debug.Log("Crouching");
                CheckCrouch();
            }
            else if (_isRunning) // Run
            {
                _currSpeed = playerRunSpeed;
                //Debug.Log("Running");
            }
            else // Walk
            {
                _currSpeed = playerWalkSpeed;
                //Debug.Log("Walking");
            }

            _charControl.height = Mathf.Lerp(_charControl.height, localHeight, crouchLerp * Time.deltaTime);
            _charControl.center = new Vector3(0, Mathf.Lerp(_charControl.center.y, localCenter, crouchLerp * Time.deltaTime), 0);
        }
        #endregion

        #endregion

        #region Events

        #region Input System
        /// <summary>
        /// Function tied with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnMove(InputAction.CallbackContext context) => _moveDirection = context.ReadValue<Vector2>();

        /// <summary>
        /// Function tied with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnCrouch(InputAction.CallbackContext context) => _isCrouchingButtonPressed = context.ReadValueAsButton();

        /// <summary>
        /// Function tied with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnCrouchToggle(InputAction.CallbackContext context)
        {
            if (context.started)
                _isCrouchingButtonPressed = !_isCrouchingButtonPressed;
        }

        /// <summary>
        /// Function tied with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnRun(InputAction.CallbackContext context)
        {
            //if (!IsCrouching)
            _isRunning = context.ReadValueAsButton();
        }
        #endregion

        #endregion
    }
}
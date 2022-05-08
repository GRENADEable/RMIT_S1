using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim_F2
{
    public class PlayerControllerBall : MonoBehaviour
    {
        #region Serialized Variables

        #region Datas
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerDataMiniGame gmData = default;
        #endregion

        #region Player Movement
        [Space, Header("Player Movement")]
        [SerializeField]
        [Tooltip("Player Speed Multiplier")]
        private float moveForceMulti = default;

        [SerializeField]
        [Tooltip("Player Speed Clamp")]
        private float forceClamp = default;

        [SerializeField]
        [Tooltip("Player Speed Multiplier")]
        private float jumpForceMulti = default;
        #endregion

        #region Player Grounding
        [Space, Header("Ground Check")]
        [SerializeField]
        [Tooltip("Spherecast radius for the ground")]
        private float sphereRadius = 0.4f;

        [SerializeField]
        [Tooltip("Which layer(s) is used for the ground")]
        private LayerMask groundMask = default;
        #endregion

        #region Events
        public delegate void SendEventsScript(PlayerControllerBall plyBall);
        /// <summary>
        /// Event sent from PlayerControllerBall to GameManagerPlatformDuel Script;
        /// Sends GameObject ref of the Player;
        /// </summary>
        public static event SendEventsScript OnPlayerIntialised;

        public delegate void SendEventsInt(int index);
        /// <summary>
        /// Event sent from PlayerControllerBall to GameManagerPlatformDuel Script;
        /// Sends PlayerIndex to disable the GameObject when Dead;
        /// </summary>
        public static event SendEventsInt OnPlayerFall;
        #endregion

        #endregion

        #region Private Variables

        #region Player Movement
        [Header("Player Movemnt")]
        private Vector3 _movement = default;
        private bool IsJumping { get => _isJumping; set => _isJumping = value; }
        private bool _isJumping = default;
        private bool CanJump { get => _canJump; set => _canJump = value; }
        private bool _canJump = default;
        private bool IsSwitchedControls { get => _isSwitchedControls; set => _isSwitchedControls = value; }
        private bool _isSwitchedControls = default;
        #endregion

        #region Player Components
        [Header("Player Components")]
        private Rigidbody _rg = default;
        public int PlayerIndex { get => _currPlayerIndex; set => _currPlayerIndex = value; }
        private int _currPlayerIndex = default;
        #endregion

        #region Player Grounding
        [Header("Ground Check")]
        private bool _isGrounded = default;
        #endregion

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            GameManagerPlatformDuel.OnControlsReversed += OnControlsSwitchedEventReceived;
            GameManagerPlatformDuel.OnControlsJump += OnControlsJumpEventReceived;
        }

        void OnDisable()
        {
            GameManagerPlatformDuel.OnControlsReversed -= OnControlsSwitchedEventReceived;
            GameManagerPlatformDuel.OnControlsJump -= OnControlsJumpEventReceived;
        }

        void OnDestroy()
        {
            GameManagerPlatformDuel.OnControlsReversed -= OnControlsSwitchedEventReceived;
            GameManagerPlatformDuel.OnControlsJump -= OnControlsJumpEventReceived;
        }
        #endregion

        void Start()
        {
            _rg = GetComponentInChildren<Rigidbody>();
            //_col = GetComponent<Collider>();
            BallIntialise();
        }

        void Update()
        {
            if (gmData.currState == GameManagerDataMiniGame.GameState.Game)
            {
                RollPlayer();
                JumpPlayer();
                GroundCheck();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Death_Box"))
                OnPlayerFall?.Invoke(PlayerIndex);
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Intialises the player ball;
        /// </summary>
        void BallIntialise()
        {
            CanJump = true;
            IsSwitchedControls = false;
            OnPlayerIntialised?.Invoke(this);
        }

        #region Player Checks
        /// <summary>
        /// Ground check for gavity and jumping;
        /// </summary>
        void GroundCheck() => _isGrounded = Physics.CheckSphere(transform.position, sphereRadius, groundMask);
        #endregion

        #region Player Movement
        /// <summary>
        /// Player Roll Movemnt;
        /// </summary>
        void RollPlayer()
        {
            float xMove = _movement.x;
            float yMove = _movement.y;
            Vector3 movement;

            if (IsSwitchedControls)
                movement = new Vector3(-xMove, 0f, -yMove).normalized;
            else
                movement = new Vector3(xMove, 0f, yMove).normalized;

            _rg.AddForce(moveForceMulti * Time.deltaTime * movement, ForceMode.Impulse);
            _rg.velocity = Vector3.ClampMagnitude(_rg.velocity, forceClamp);
        }

        /// <summary>
        /// Player Ball Jump;
        /// </summary>
        void JumpPlayer()
        {
            if (_isGrounded && IsJumping && CanJump)
            {
                IsJumping = false;
                _rg.AddForce(Vector3.up * jumpForceMulti, ForceMode.Impulse);
            }
        }
        #endregion

        #endregion

        #region Events

        #region Input System
        /// <summary>
        /// Tied to new Input System;
        /// Reads the Vector 2D Input from the Player for Movement;
        /// </summary>
        /// <param name="context"> Parameter from the new Input System; </param>
        public void OnMovePlayer(InputAction.CallbackContext context) => _movement = context.ReadValue<Vector2>();

        /// <summary>
        /// Tied to new Input System;
        /// Reads the Button Input from the Player for Jumping;
        /// </summary>
        /// <param name="context"> Parameter from the new Input System; </param>
        public void OnJumpPlayer(InputAction.CallbackContext context) => IsJumping = context.ReadValueAsButton();
        #endregion

        /// <summary>
        /// Subbed to event from GameManagerPlatformDuel Script;
        /// Inverts the controls of the players;
        /// </summary>
        /// <param name="isSwitch"> If True, switch Controls and vice versa; </param>
        void OnControlsSwitchedEventReceived(bool isSwitch)
        {
            if (isSwitch)
                IsSwitchedControls = true;
            else
                IsSwitchedControls = false;
        }

        /// <summary>
        /// Subbed to event from GameManagerPlatformDuel Script;
        /// Stops the player from Jumping;
        /// </summary>
        /// <param name="isJumping"> If True, Player can jump and vice versa; </param>
        void OnControlsJumpEventReceived(bool isJumping)
        {
            if (isJumping)
                CanJump = true;
            else
                CanJump = false;
        }
        #endregion
    }
}
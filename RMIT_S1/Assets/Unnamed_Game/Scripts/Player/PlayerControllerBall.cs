using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim_F2
{
    public class PlayerControllerBall : MonoBehaviour
    {
        #region Serialized Variables

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

        #endregion

        #region Private Variables

        #region Player Movement
        [Header("Player Movemnt")]
        private Vector3 _movement = default;
        private bool IsJumping { get => _isJumping; set => _isJumping = value; }
        private bool _isJumping = default;
        private bool _canJump = default;
        private bool IsSwitchedControls { get => _isSwitchedControls; set => _isSwitchedControls = value; }
        private bool _isSwitchedControls = default;
        #endregion

        #region Player Components
        [Header("Player Components")]
        private Rigidbody _rg = default;
        //private Collider _col = default;
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
            GameManagerPlatformDuel.OnControlsSwitched += OnControlsSwitchedEventReceived;
            GameManagerPlatformDuel.OnControlsJump += OnControlsJumpEventReceived;
        }

        void OnDisable()
        {
            GameManagerPlatformDuel.OnControlsSwitched -= OnControlsSwitchedEventReceived;
            GameManagerPlatformDuel.OnControlsJump -= OnControlsJumpEventReceived;
        }

        void OnDestroy()
        {
            GameManagerPlatformDuel.OnControlsSwitched -= OnControlsSwitchedEventReceived;
            GameManagerPlatformDuel.OnControlsJump -= OnControlsJumpEventReceived;
        }
        #endregion

        void Start()
        {
            _rg = GetComponentInChildren<Rigidbody>();
            //_col = GetComponent<Collider>();
        }

        void Update()
        {
            RollPlayer();
            JumpPlayer();
            GroundCheck();
        }
        #endregion

        #region My Functions

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
                movement = new Vector3(xMove, 0f, yMove).normalized;
            else
                movement = new Vector3(-xMove, 0f, -yMove).normalized;

            _rg.AddForce(moveForceMulti * Time.deltaTime * movement, ForceMode.Impulse);
            _rg.velocity = Vector3.ClampMagnitude(_rg.velocity, forceClamp);
        }

        /// <summary>
        /// Player Ball Jump;
        /// </summary>
        void JumpPlayer()
        {
            if (_isGrounded && IsJumping && _canJump)
            {
                IsJumping = false;
                _rg.AddForce(Vector3.up * jumpForceMulti, ForceMode.Impulse);
                Debug.Log("Jumping");
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

        //public void OnControlsGained()
        //{
        //    _rg.isKinematic = false;
        //    _col.isTrigger = false;
        //}

        //public void OnControlsLost()
        //{
        //    _rg.isKinematic = true;
        //    _col.isTrigger = true;
        //}
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
                _canJump = true;
            else
                _canJump = false;
        }
        #endregion
    }
}
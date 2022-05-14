using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim_F2
{
    public class PlayerControllerCapsule : MonoBehaviour
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
        [Tooltip("Player Speed")]
        private float playerWalkSpeed = default;

        [SerializeField]
        [Tooltip("Player Running Speed")]
        private float playerRunSpeed = default;

        [SerializeField]
        [Tooltip("Player Rotation Speed")]
        [Range(0f, 15f)] private float playerRotationSpeed = default;

        [SerializeField]
        [Tooltip("Power of how high the player can jump")]
        private float jumpPower = 2f;
        #endregion

        #region Bomber Variables
        [Space, Header("Bomber Variables")]
        [SerializeField]
        [Tooltip("Self Bomber flag disable delay")]
        private float selfBomberFlagDelay = default;
        #endregion

        #region Player Grounding
        [Space, Header("Ground Check")]
        [SerializeField]
        [Tooltip("Gravity of the player when falling")]
        private float gravity = -19.62f;

        [SerializeField]
        [Tooltip("Spherecast radius for the ground")]
        private float sphereRadius = 0.4f;

        [SerializeField]
        [Tooltip("Ground Transform Position")]
        private Transform groundPos;

        [SerializeField]
        [Tooltip("Which layer(s) is used for the ground")]
        private LayerMask groundMask = default;
        #endregion

        #region Events
        public delegate void SendEventsScript(PlayerControllerCapsule plyCapsule);
        /// <summary>
        /// Event sent from PlayerControllerBall to GameManagerPlatformDuel Script;
        /// Sends GameObject ref of the Player;
        /// </summary>
        public static event SendEventsScript OnPlayerIntialised;

        public delegate void SendEventsInt(int index);
        ///// <summary>
        ///// Event sent from PlayerControllerBall to GameManagerPlatformDuel Script;
        ///// Sends PlayerIndex to disable the GameObject when Dead;
        ///// </summary>
        //public static event SendEventsInt OnPlayerFall;

        public static event SendEventsInt OnPlayerPassBomb;

        public delegate void SendEvents();
        /// <summary>
        /// Event sent from PlayerControllerBall to GameManagerPlatformDuel Script;
        /// Sends event to pause game;
        /// </summary>
        public static event SendEvents OnGamePaused;
        #endregion

        #endregion

        #region Private Variables

        #region Player Movement
        [Header("Player Movemnt")]
        private Vector3 _movement = default;
        private bool IsJumping { get => _isJumping; set => _isJumping = value; }
        [SerializeField] private bool _isJumping = default;
        private Vector3 _vel = default;
        [SerializeField] private float _currSpeed = default;
        private bool CanJump { get => _canJump; set => _canJump = value; }
        private bool _canJump = default;
        #endregion

        #region Player Components
        [Header("Player Components")]
        private CharacterController _charControl = default;
        private CapsuleCollider _charCol = default;
        public int PlayerIndex { get => _currPlayerIndex; set => _currPlayerIndex = value; }
        [SerializeField] private int _currPlayerIndex = default;

        public bool PlayerBomber { get => _isBomber; set => _isBomber = value; }
        [SerializeField] private bool _isBomber = default;
        #endregion

        [Header("Ground Check")]
        private bool _isGrounded = default;

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            GameManagerHotPotato.OnControlsJump += OnControlsJumpEventReceived;
            GameManagerHotPotato.OnControlsSpeed += OnControlsSpeedEventReceived;
            GameManagerHotPotato.OnBombChoose += OnBombChooseEventReceived;
        }

        void OnDisable()
        {
            GameManagerHotPotato.OnControlsJump -= OnControlsJumpEventReceived;
            GameManagerHotPotato.OnControlsSpeed -= OnControlsSpeedEventReceived;
            GameManagerHotPotato.OnBombChoose -= OnBombChooseEventReceived;

            CanJump = true;
        }

        void OnDestroy()
        {
            GameManagerHotPotato.OnControlsJump -= OnControlsJumpEventReceived;
            GameManagerHotPotato.OnControlsSpeed -= OnControlsSpeedEventReceived;
            GameManagerHotPotato.OnBombChoose -= OnBombChooseEventReceived;
        }
        #endregion

        void Start()
        {
            _charControl = GetComponent<CharacterController>();
            _charCol = GetComponent<CapsuleCollider>();
            CapsuleIntialise();
        }

        void Update()
        {
            MoveRotatePlayer();
            GroundCheck();
        }

        void OnTriggerEnter(Collider other)
        {
            //if (other.CompareTag("Death_Box"))
            //    OnPlayerFall?.Invoke(PlayerIndex);

            if (other.CompareTag("Player"))
            {
                if (other.GetComponent<PlayerControllerCapsule>() != null &&
                    !other.GetComponent<PlayerControllerCapsule>().PlayerBomber)
                {
                    OnPlayerPassBomb?.Invoke(other.GetComponent<PlayerControllerCapsule>().PlayerIndex);
                    _charCol.enabled = false;
                    StartCoroutine(BomberPassDelay());
                    Debug.Log("Sending Event");
                }
            }
        }

        //void OnControllerColliderHit(ControllerColliderHit other)
        //{
        //    if (other.collider.CompareTag("Player"))
        //        Debug.Log($"Hitting {other.collider.name}");
        //}
        #endregion

        #region My Functions
        /// <summary>
        /// Intialises the player ball;
        /// </summary>
        void CapsuleIntialise()
        {
            CanJump = true;
            OnPlayerIntialised?.Invoke(this);
            _currSpeed = playerWalkSpeed;
        }

        #region Player Checks
        /// <summary>
        /// Ground check for gavity and jumping;
        /// </summary>
        void GroundCheck()
        {
            _isGrounded = Physics.CheckSphere(groundPos.position, sphereRadius, groundMask);

            if (_isGrounded && _vel.y < 0)
            {
                IsJumping = false;
                _vel.y = -2f;
            }

            _vel.y += gravity * Time.deltaTime;
            _charControl.Move(_vel * Time.deltaTime);
        }
        #endregion

        #region Player Movement
        /// <summary>
        /// Player Roll Movemnt;
        /// </summary>
        void MoveRotatePlayer()
        {
            Vector3 movement;

            //if (IsSwitchedControls)
            //    movement = new Vector3(-xMove, 0f, -yMove).normalized;
            //else
            movement = new Vector3(_movement.x, 0f, _movement.y).normalized;

            if (movement != Vector3.zero)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movement), playerRotationSpeed * Time.deltaTime);

            _charControl.Move(_currSpeed * Time.deltaTime * movement);
        }

        /// <summary>
        /// Player Ball Jump;
        /// </summary>
        void JumpPlayer()
        {
            if (_isGrounded && !IsJumping && CanJump)
            {
                IsJumping = true;
                float jumpForce = Mathf.Sqrt(jumpPower * Mathf.Abs(gravity) * 2);
                _vel.y += jumpForce;
            }
        }
        #endregion

        #endregion

        #region Coroutines
        IEnumerator BomberPassDelay()
        {
            yield return new WaitForSeconds(selfBomberFlagDelay);
            _isBomber = false;
        }
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
        public void OnJumpPlayer(InputAction.CallbackContext context)
        {
            if (gmData.currState == GameManagerDataMiniGame.GameState.Game)
            {
                IsJumping = context.ReadValueAsButton();
                JumpPlayer();
            }
        }

        /// <summary>
        /// Tied to new Input System;
        /// Readds the Button Input from the player for Pausing;
        /// </summary>
        /// <param name="context"> Parameter from the new Input System; </param>
        public void OnPlayerPause(InputAction.CallbackContext context)
        {
            if (gmData.currState != GameManagerDataMiniGame.GameState.Paused &&
                gmData.currState != GameManagerDataMiniGame.GameState.Starting)
            {
                if (context.started)
                    OnGamePaused?.Invoke();
            }
        }
        #endregion

        #region Player Control Obstacles
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

        /// <summary>
        /// Subbed to event from GameManagerPlatformDuel Script;
        /// Increased the Player's Speed;
        /// </summary>
        /// <param name="isSpeeding"> If True, Player can moves fast and vice versa; </param>
        void OnControlsSpeedEventReceived(bool isSpeeding)
        {
            if (isSpeeding)
                _currSpeed = playerRunSpeed;
            else
                _currSpeed = playerWalkSpeed;
        }
        #endregion

        #region Game
        void OnBombChooseEventReceived(int bombIndex)
        {
            if (bombIndex == PlayerIndex)
            {
                _charCol.enabled = true;
                _isBomber = true;
            }
        }
        #endregion

        #endregion
    }
}
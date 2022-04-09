using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim
{
    public class HideInBed : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Datas")]
        [SerializeField]
        [Tooltip("GameManager Data ScriptableObject")]
        private GameManagerData gmData = default;

        [Space, Header("Cameras")]
        [SerializeField]
        [Tooltip("Camera Player GameObject")]
        private GameObject camPlayer = default;

        [SerializeField]
        [Tooltip("Camera Bed GameObject")]
        private GameObject camUnderBed = default;

        [Space, Header("Mouse Settings")]
        [SerializeField]
        [Tooltip("Mouse sensitivity")]
        private float mouseSens = 45f;

        [Space, Header("Transforms")]
        [SerializeField]
        private Transform playerOrientation = default;

        #region Events
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from HideInBed Script to FlashLight Script;
        /// Just disables the Flashlight when the Player is hiding under the bed;
        /// </summary>
        public static event SendEvents OnPlayerHidingUnderBed;
        #endregion

        #endregion

        #region Private Variables
        private Vector2 _lookInput = default;
        [SerializeField] private bool _isHidingInBed = default;
        #endregion

        #region Unity Callbacks
        void Update()
        {
            if (gmData.currState == GameManagerData.GameState.Hidden)
                LookAround();
        }
        #endregion

        #region My Functions
        public void HideInThisBed()
        {
            camPlayer.transform.parent.gameObject.transform.SetPositionAndRotation(playerOrientation.position, playerOrientation.rotation);

            camPlayer.SetActive(false);
            camUnderBed.SetActive(true);
            _isHidingInBed = true;

            gmData.ChangeGameState("Hidden");

            OnPlayerHidingUnderBed?.Invoke();
            //Debug.Log("Hiding in Bed");
        }

        void LookAround()
        {
            float mouseX = _lookInput.x * mouseSens * Time.deltaTime;
            camUnderBed.transform.Rotate(Vector3.up * mouseX);
        }
        #endregion

        #region Events
        /// <summary>
        /// Function tied with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnMouseLookBed(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

        /// <summary>
        /// Function tied with PlayerInput from the new Input Systems;
        /// </summary>
        /// <param name="context"> Parameter Checks if the button is pressed or not; </param>
        public void OnBedGetUp(InputAction.CallbackContext context)
        {
            if (context.started && gmData.currState == GameManagerData.GameState.Hidden && _isHidingInBed)
            {
                camPlayer.SetActive(true);
                camUnderBed.SetActive(false);
                _isHidingInBed = false;
                gmData.ChangeGameState("Game");
                //Debug.Log("Got out of Bed");
            }
        }
        #endregion
    }
}
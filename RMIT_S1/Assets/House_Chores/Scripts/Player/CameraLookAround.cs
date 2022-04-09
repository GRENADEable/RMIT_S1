using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim
{
    public class CameraLookAround : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerData gmData = default;

        [SerializeField]
        [Tooltip("Transform Component of the root object")]
        private Transform playerRoot = default;

        [Space, Header("Mouse Settings")]
        [SerializeField]
        [Tooltip("Minimum clamp on X Axis")]
        private float minXClamp = -90f;

        [SerializeField]
        [Tooltip("Maximum clamp on X Axis")]
        private float maxXClamp = 90f;

        [SerializeField]
        [Tooltip("Mouse sensitivity for PC")]
        private float mouseSensPC = 45f;

        [SerializeField]
        [Tooltip("Mouse sensitivity for Mobile")]
        private float mouseSensMobile = 5;
        #endregion

        #region Private Variables
        private float _xRotate = default;
        private Vector2 _lookInput = default;
        #endregion

        #region Unity Callbacks
        void Update()
        {
            if (gmData.currState == GameManagerData.GameState.Game)
                LookAround();
        }
        #endregion

        #region My Functions
        void LookAround()
        {
#if  UNITY_STANDALONE
            float mouseX = _lookInput.x * mouseSensPC * Time.deltaTime;
            float mouseY = _lookInput.y * mouseSensPC * Time.deltaTime;
#else
            float mouseX = _lookInput.x * mouseSensMobile * Time.deltaTime;
            float mouseY = _lookInput.y * mouseSensMobile * Time.deltaTime;
#endif
            _xRotate -= mouseY;
            _xRotate = Mathf.Clamp(_xRotate, minXClamp, maxXClamp);

            transform.localRotation = Quaternion.Euler(_xRotate, 0f, 0f);

            playerRoot.Rotate(Vector3.up * mouseX);
        }
        #endregion

        #region Events
        public void OnMouseLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();
        #endregion
    }
}
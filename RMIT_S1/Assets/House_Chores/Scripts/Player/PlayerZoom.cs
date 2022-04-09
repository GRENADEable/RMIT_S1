using UnityEngine;
using UnityEngine.InputSystem;

namespace Khatim
{
    public class PlayerZoom : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameManagerData gmData = default;

        #region Player Zoom
        [Space, Header("Player Zoom")]
        [SerializeField]
        [Tooltip("How much FoV will it zoom into")]
        private float zoomFovVal = 25f;

        [Range(0f, 10f)]
        [SerializeField]
        [Tooltip("Zoom lerp speed")]
        private float lerpTime = 10f;
        #endregion

        #endregion

        #region Private Variables
        [Header("Player Zoom")]
        private float _currZoomFov;
        private Camera _cam;
        private bool _isZooming;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            _cam = Camera.main;
            _currZoomFov = _cam.fieldOfView;
        }

        void Update()
        {
            if (gmData.currState == GameManagerData.GameState.Game)
                PlayerZooming();
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Zoom using the player's Camera FoV;
        /// </summary>
        void PlayerZooming()
        {
            if (_isZooming)
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, zoomFovVal, lerpTime * Time.deltaTime);
            else
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _currZoomFov, lerpTime * Time.deltaTime);
        }
        #endregion

        #region Events
        public void OnZoom(InputAction.CallbackContext context) => _isZooming = context.ReadValueAsButton();
        #endregion
    }
}
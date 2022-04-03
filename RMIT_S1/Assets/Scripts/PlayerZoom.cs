using UnityEngine;

    public class PlayerZoom : MonoBehaviour
    {
        #region Serialized Variables
        #region Player Zoom
        [Space, Header("Player Zoom")]
        [SerializeField]
        [Tooltip("Which key to press when zooming")]
        private KeyCode zoomKey = KeyCode.Mouse1;

        [SerializeField]
        [Tooltip("How much FoV will it zoom into")]
        private float zoomFovVal = 25f;

        [Range(0f, 10f)]
        [SerializeField]
        [Tooltip("Zoom lerp speed")]
        private float lerpTime = 10f;
        #endregion

        #region Events
        public delegate void SendEventsBool(bool isZooming);
        /// <summary>
        /// Event sent from FPSController to FPSDefaultUI Scripts;
        /// This event just changes the variables when player zooms in or out;
        /// </summary>
        public static event SendEventsBool OnZoomInCam;
        #endregion

        #endregion

        #region Private Variables

        #region Player Zoom
        [Header("Player Zoom")]
        private float _currZoomFov;
        private Camera _cam;
        #endregion

        #endregion

        #region Unity Callbacks
        void Start()
        {
            _cam = Camera.main;
            _currZoomFov = _cam.fieldOfView;
        }

        void Update() => PlayerZooming();
        #endregion

        #region My Functions
        /// <summary>
        /// Zoom using the player's Camera FoV;
        /// </summary>
        void PlayerZooming()
        {
            if (Input.GetKey(zoomKey))
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, zoomFovVal, lerpTime * Time.deltaTime);
            else
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _currZoomFov, lerpTime * Time.deltaTime);

            // Placed a single frame key down for the event to be sent to the FPSDefaultUI Script;
            if (Input.GetKeyDown(zoomKey))
                OnZoomInCam?.Invoke(true);
            else if (Input.GetKeyUp(zoomKey))
                OnZoomInCam?.Invoke(false);
        }
        #endregion
    }
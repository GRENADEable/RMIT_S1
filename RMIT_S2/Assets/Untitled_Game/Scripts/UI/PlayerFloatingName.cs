using UnityEngine;
using TMPro;

namespace Khatim_F2
{
    public class PlayerFloatingName : MonoBehaviour
    {
        #region Serialized Fields
        [Space, Header("UI")]
        [SerializeField]
        [Tooltip("How much does the UI Offset?")]
        private Vector3 uiOffset = default;
        #endregion

        #region Private Variables
        private Camera _cam;
        public Transform FollowPos { get => _followPos; set => _followPos = value; }
        private Transform _followPos = default;

        public TextMeshProUGUI FloatingNameText { get => _floatingNameText; set => _floatingNameText = value; }
        private TextMeshProUGUI _floatingNameText = default;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            _cam = Camera.main;
            _floatingNameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        void LateUpdate()
        {
            transform.position = FollowPos.transform.position + uiOffset;
            transform.LookAt(_cam.transform.position);
        }
        #endregion
    }
}
using UnityEngine;

namespace Khatim
{
    public class ScreenImageManager : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Image Files")]
        private Texture2D[] screenImgs = default;

        [SerializeField]
        [Tooltip("TV Screen Mesh")]
        private MeshRenderer tvMesh = default;

        [SerializeField]
        [Tooltip("Stove Glass Mesh")]
        private MeshRenderer stoveScreenMesh = default;

        #region Events
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from ScreenImageManager to GameManager Script;
        /// Changes to the 7th Objective;
        /// </summary>
        public static event SendEvents OnShowObj7;
        #endregion

        #endregion

        #region Private Variables
        private int _currImgIndex = default;
        #endregion

        #region Unity Callbacks

        void OnEnable()
        {
            TVRemote.OnChannelChange += OnChannelChangeEventReceived;
        }

        void OnDisable()
        {
            TVRemote.OnChannelChange -= OnChannelChangeEventReceived;
        }

        void OnDestroy()
        {
            TVRemote.OnChannelChange -= OnChannelChangeEventReceived;
        }

        #endregion

        #region My Functions
        public void OnToggleScreen()
        {
            tvMesh.gameObject.SetActive(!tvMesh.gameObject.activeSelf);
            stoveScreenMesh.gameObject.SetActive(!stoveScreenMesh.gameObject.activeSelf);

            if (tvMesh.gameObject.activeInHierarchy)
                OnShowObj7?.Invoke();
        }

        void ChangeImage(int index)
        {
            tvMesh.material.EnableKeyword("_EMISSION");
            tvMesh.material.SetTexture("_EmissionMap", screenImgs[index]);

            stoveScreenMesh.material.EnableKeyword("_EMISSION");
            stoveScreenMesh.material.SetTexture("_EmissionMap", screenImgs[index]);
        }
        #endregion

        #region Events
        void OnChannelChangeEventReceived()
        {
            _currImgIndex++;

            if (_currImgIndex >= screenImgs.Length)
            {
                _currImgIndex = 0;
                ChangeImage(_currImgIndex);
            }
            else
                ChangeImage(_currImgIndex);
        }
        #endregion
    }
}
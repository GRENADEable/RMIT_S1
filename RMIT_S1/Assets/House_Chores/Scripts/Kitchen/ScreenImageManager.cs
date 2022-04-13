using System.Collections;
using System.Collections.Generic;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Khatim
{
    public class PropHighlight : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("If this Object has Children GameObjects")]
        private bool hasChildren = default;

        [SerializeField]
        [Tooltip("If this Object has Root Mesh")]
        private bool hasRootMesh = default;

        [SerializeField]
        [Tooltip("GameObject Array References")]
        private GameObject[] childrenObjs = default;
        #endregion

        #region Private Variables
        private const string _emissive = "_EMISSION";
        private Material _mat;
        #endregion

        #region Unity Callbacks
        void Start() => DisableEmissions();
        #endregion

        #region My Functions
        /// <summary>
        /// Highlights depending on the bool;
        /// </summary>
        /// <param name="isHighlighted"> If true, highlight, if false, don't highlight; </param>
        public void HighLightObject(bool isHighlighted)
        {
            if (isHighlighted)
            {
                if (hasRootMesh)
                    _mat.EnableKeyword(_emissive);

                if (hasChildren)
                {
                    foreach (GameObject childObj in childrenObjs)
                    {
                        Material mat = childObj.GetComponent<Renderer>().material;
                        mat.EnableKeyword(_emissive);
                    }
                }
            }
            else
                DisableEmissions();
        }

        /// <summary>
        /// Disables all the Emission on the Mesh object itself;
        /// </summary>
        void DisableEmissions()
        {
            if (hasRootMesh)
            {
                _mat = gameObject.GetComponent<Renderer>().material;
                _mat.DisableKeyword(_emissive);
            }

            if (hasChildren)
            {
                foreach (GameObject childObj in childrenObjs)
                {
                    Material mat = childObj.GetComponent<Renderer>().material;
                    mat.DisableKeyword(_emissive);
                }
            }
        }
        #endregion
    }
}
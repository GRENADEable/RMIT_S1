using System.Collections.Generic;
using UnityEngine;

namespace MadInc
{
    public class LightFlickering : MonoBehaviour
    {
        #region Serialized Variables
        [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
        [SerializeField]
        private Light flashLight;

        [Tooltip("Minimum random light intensity")]
        [SerializeField]
        private float minIntensity = 0f;

        [Tooltip("Maximum random light intensity")]
        [SerializeField]
        private float maxIntensity = 1.2f;

        [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
        [SerializeField]
        [Range(1, 50)]
        private int smoothing = 5;
        #endregion

        #region Private Variables
        private Queue<float> _smoothQueue;
        private float _lastSum = 0;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            _smoothQueue = new Queue<float>(smoothing);

            flashLight = GetComponent<Light>();
        }

        void Reset()
        {
            _smoothQueue.Clear();
            _lastSum = 0;
        }

        void Update()
        {
            if (flashLight == null)
                return;

            FlashLightFlicker();
        }
        #endregion

        #region My Functions
        void FlashLightFlicker()
        {
            // pop off an item if too big
            while (_smoothQueue.Count >= smoothing)
                _lastSum -= _smoothQueue.Dequeue();

            // Generate random new item, calculate new average
            float newVal = Random.Range(minIntensity, maxIntensity);
            _smoothQueue.Enqueue(newVal);
            _lastSum += newVal;

            // Calculate new smoothed average
            flashLight.intensity = _lastSum / (float)_smoothQueue.Count;
        }
        #endregion
    }
}
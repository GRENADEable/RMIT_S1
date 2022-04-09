using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Khatim
{
    public class AudioManager : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Audio Source OneShot ")]
        private AudioSource audOneShot;

        [SerializeField]
        [Tooltip("Audio SFX Clips")]
        private AudioClip[] sfxClips;
        #endregion

        #region Private Variables

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            EnemyFSM.OnPlayerChaseStarted += OnPlayerChaseStartedEventReceived;
            EnemyFSM.OnPlayerChaseEnded += OnPlayerChaseEndedEventReceived;
        }

        void OnDisable()
        {
            EnemyFSM.OnPlayerChaseStarted -= OnPlayerChaseStartedEventReceived;
            EnemyFSM.OnPlayerChaseEnded -= OnPlayerChaseEndedEventReceived;
        }

        void OnDestroy()
        {
            EnemyFSM.OnPlayerChaseStarted -= OnPlayerChaseStartedEventReceived;
            EnemyFSM.OnPlayerChaseEnded -= OnPlayerChaseEndedEventReceived;
        }
        #endregion

        void Start()
        {

        }

        void Update()
        {

        }
        #endregion

        #region My Functions
        void AudioAcess(int value) => audOneShot.PlayOneShot(sfxClips[value]);

        void AudioOneShotStop() => audOneShot.Stop();
        #endregion

        #region Coroutines

        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from EnemyFSM Script;
        /// This just plays the chase start audio and stops the chase end audio;
        /// </summary>
        void OnPlayerChaseStartedEventReceived()
        {
            AudioOneShotStop();
            AudioAcess(0);
        }

        /// <summary>
        /// Subbed to event from EnemyFSM Script;
        /// This just plays the chase end audio and stops the chase start audio;
        /// </summary>
        void OnPlayerChaseEndedEventReceived()
        {
            AudioOneShotStop();
            //AudioAcess(1);
        }
        #endregion
    }
}
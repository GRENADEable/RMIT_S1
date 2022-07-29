using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Khatim_F2
{
    public class CrowdManager : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Crowd Animator Components")]
        private Animator[] crowdsAnim = default;

        [SerializeField]
        [Tooltip("Crowd Bod Colours")]
        private MeshRenderer[] crowdsMesh = default;

        [SerializeField]
        [Tooltip("Total Crowd Animations")]
        private int totalcrowdIndex = default;

        [SerializeField]
        [Tooltip("Animation Delay")]
        private int animDelay = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => GameManagerPlatformDuel.OnPogAnim += OnPogAnimEventReceived;

        void OnDisable() => GameManagerPlatformDuel.OnPogAnim -= OnPogAnimEventReceived;

        void OnDestroy() => GameManagerPlatformDuel.OnPogAnim -= OnPogAnimEventReceived;
        #endregion

        void Start()
        {
            StartCoroutine(PlayRandomAnim());
            SetCrowdColours();
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Sets the crowds body colour;
        /// </summary>
        void SetCrowdColours()
        {
            for (int i = 0; i < crowdsMesh.Length; i++)
                crowdsMesh[i].material.SetColor("_Color", Random.ColorHSV());
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Plays random Animation with delay. Infinite Coroutine;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator PlayRandomAnim()
        {
            for (int i = 0; i < crowdsAnim.Length; i++)
            {
                crowdsAnim[i].SetInteger("ClapIndex", Random.Range(0, totalcrowdIndex));
                crowdsAnim[i].SetTrigger("Clap");
            }

            yield return new WaitForSeconds(animDelay);
            StartCoroutine(PlayRandomAnim());
        }

        /// <summary>
        /// Plays the animation when all the players are reset;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator PlayPogAnim()
        {
            for (int i = 0; i < crowdsAnim.Length; i++)
            {
                crowdsAnim[i].SetInteger("ClapIndex", 5);
                crowdsAnim[i].SetTrigger("Clap");
            }
            yield return new WaitForSeconds(4);
            StartCoroutine(PlayRandomAnim());
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from GameManagerPlatformDuel Script;
        /// Stops all coroutines and plays the Pog Animation;
        /// </summary>
        void OnPogAnimEventReceived()
        {
            StopAllCoroutines();
            StartCoroutine(PlayPogAnim());
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Khatim
{
    public class ToastObjective : MonoBehaviour
    {
        #region Serialized Variables

        #region Toaster Variables
        [SerializeField]
        [Tooltip("Transform Move Component")]
        private Transform[] movePos = default;

        [SerializeField]
        [Tooltip("Move Speed")]
        private float moveSpeed = default;

        [SerializeField]
        [Tooltip("Move Speed")]
        private float rotationSpeed = default;

        [SerializeField]
        [Tooltip("Min Distance to Point")]
        private float minDistance = default;

        [SerializeField]
        [Tooltip("Brot bake Timer")]
        private float brotBakeTime = default;
        #endregion

        #region Brot Variables
        [SerializeField]
        [Tooltip("Brot Prefab")]
        private GameObject brotPrefab = default;

        [SerializeField]
        [Tooltip("Brot Shoot Force")]
        private float shootForce = default;

        [SerializeField]
        [Tooltip("Brot Spawn Positions")]
        private Transform[] brotSpawnPos = default;
        #endregion

        #region Events
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from ToastObjective to GameManager Script;
        /// Updates the Objective to 4;
        /// </summary>
        public static event SendEvents OnShowObj4;

        /// <summary>
        /// Event sent from ToastObjective to GameManager Script;
        /// Updates the Objective to 5;
        /// </summary>
        public static event SendEvents OnShowObj5;
        #endregion

        #endregion

        #region Private Variables
        private int _currPos = default;
        private bool isMoving;
        private float distance = default;
        private int _currBrots = default;
        private List<GameObject> brots = new List<GameObject>();
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            PropHolder.OnToasterRun += OnToasterRunEventReceived;

            ToastReceiver.OnBrotAdded += OnBrotAddedEventReceived;
        }

        void OnDisable()
        {
            PropHolder.OnToasterRun -= OnToasterRunEventReceived;

            ToastReceiver.OnBrotAdded -= OnBrotAddedEventReceived;
        }

        void OnDestroy()
        {
            PropHolder.OnToasterRun -= OnToasterRunEventReceived;

            ToastReceiver.OnBrotAdded -= OnBrotAddedEventReceived;
        }
        #endregion

        void Start() => isMoving = false;

        void Update()
        {
            DistanceCheck();

            if (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePos[_currPos].position, moveSpeed * Time.deltaTime);
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            }

        }
        #endregion

        #region My Functions
        void DistanceCheck()
        {
            distance = Vector3.Distance(transform.position, movePos[_currPos].position);

            if (distance <= minDistance)
            {
                _currPos++;

                if (_currPos >= movePos.Length)
                    _currPos = 0;
            }
        }

        void ShootBrot()
        {
            for (int i = 0; i < brots.Count; i++)
                Destroy(brots[i]);

            for (int i = 0; i < brotSpawnPos.Length; i++)
            {
                GameObject donutObj = Instantiate(brotPrefab, brotSpawnPos[i].position, brotSpawnPos[i].rotation);
                donutObj.GetComponent<Rigidbody>().AddForce(donutObj.transform.forward * shootForce, ForceMode.Impulse);
            }
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Adds baking delay before shooting the bread out;
        /// </summary>
        /// <returns> Float Delay; </returns>
        IEnumerator BrotBake()
        {
            yield return new WaitForSeconds(brotBakeTime);
            OnShowObj5?.Invoke();
            ShootBrot();
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from PropHolder;
        /// Activates the toaster to move around;
        /// </summary>
        /// <param name="isRunning"></param>
        void OnToasterRunEventReceived(bool isRunning)
        {
            if (isRunning)
                isMoving = true;
            //else
            //    isMoving = false;
        }

        /// <summary>
        /// Subbed to event from ToastReceiver;
        /// Increments the total brot counter by 1;
        /// </summary>
        /// <param name="obj"> Brot GameObject </param>
        void OnBrotAddedEventReceived(GameObject obj)
        {
            brots.Add(obj);

            _currBrots++;

            if (_currBrots == 1)
                OnShowObj4?.Invoke();

            if (_currBrots >= 2)
                StartCoroutine(BrotBake());
        }
        #endregion
    }
}
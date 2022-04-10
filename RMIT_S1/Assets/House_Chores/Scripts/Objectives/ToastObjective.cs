using System.Collections;
using UnityEngine;

namespace Khatim
{
    public class ToastObjective : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Toaster Cooking Time")]
        private float toastDelay = 2f;

        [Space, Header("Donut Variables")]
        [SerializeField]
        [Tooltip("Donut GameObject")]
        private GameObject donutPrefab = default;

        [SerializeField]
        [Tooltip("Donut Shoot Force")]
        private float donutForce = 1f;

        [SerializeField]
        [Tooltip("Donut Spawn Positions")]
        private Transform[] donutSpawnPos = default;
        #endregion

        #region Private Variables
        private SwingController _swController = default;
        private DoorTrigger _door = default;
        private bool _isTriggered = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {

        }

        void OnDisable()
        {

        }

        void OnDestroy()
        {

        }
        #endregion

        void Start()
        {
            _swController = GetComponent<SwingController>();
            _door = GetComponent<DoorTrigger>();
        }

        void Update()
        {
            if (_swController.IsDoorOpened() && !_isTriggered)
            {
                _isTriggered = true;
                StartCoroutine(ToastBreadDelay());
                gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Spawns the donut when the coroutine ends;
        /// </summary>
        void SpawnDonut()
        {
            for (int i = 0; i < donutSpawnPos.Length; i++)
            {
                GameObject donutObj = Instantiate(donutPrefab, donutSpawnPos[i].position, donutSpawnPos[i].rotation);
                donutObj.GetComponent<Rigidbody>().AddForce(donutObj.transform.forward * donutForce, ForceMode.Impulse);
            }
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Delays the toaster button;
        /// </summary>
        /// <returns> Float delay; </returns>
        IEnumerator ToastBreadDelay()
        {
            yield return new WaitForSeconds(toastDelay);
            _door.InteractDoor(gameObject);
            SpawnDonut();
        }
        #endregion

        #region Events

        #endregion
    }
}
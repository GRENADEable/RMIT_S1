using UnityEngine;

namespace MadInc
{
    public class SwingController : MonoBehaviour
    {
        #region Serialized Variables

        #region Door Enums
        [Space, Header("Door Enums")]
        [SerializeField]
        [Tooltip("Current door type. Will it rotate? Move or Both?")]
        private DoorType currType = DoorType.Rotating;
        private enum DoorType { Moving, Rotating, Both };
        #endregion

        [SerializeField]
        [Tooltip("This iD is being used for door Trigger. Has to be the same id as the DoorTrigger Script which is attached to the parent of this object.")]
        private int triggerID = default;

        #region Door GameObjects
        [Space, Header("Door Gamobjects")]
        [SerializeField]
        [Tooltip("Place the Door Gameobject which will be visible in the scene")]
        private GameObject doorClose = default;

        [SerializeField]
        [Tooltip("Place the opened Door Gameobject")]
        private GameObject doorOpen = default;
        #endregion

        #region Door Variables
        [Space, Header("Door Variables")]
        [SerializeField]
        [Tooltip("Door movement speed")]
        private float moveSpeed = 3f;

        [SerializeField]
        [Tooltip("Door rotation speed")]
        private float rotationSpeed = 90f;

        [SerializeField]
        [Tooltip("Is the door Opened? Or Closed?")]
        private bool _isOpened = false;
        public bool _isOnHold = false;
        #endregion

        #region Door Audio
        [Space, Header("Door Audio")]
        [SerializeField]
        [Tooltip("Audio Source on th Door")]
        private AudioSource audDoor;

        [SerializeField]
        [Tooltip("Audio SFX Close")]
        private AudioClip sfxDoorClose;

        [SerializeField]
        [Tooltip("Audio SFX Open")]
        private AudioClip sfxDoorOpen;
        #endregion

        #endregion

        #region Private Variables
        private GameObject _doorCloseObj = default;

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => DoorTrigger.OnDoorTrigger += OnDoorTriggerEventReceived;

        void OnDisable() => DoorTrigger.OnDoorTrigger -= OnDoorTriggerEventReceived;

        void OnDestroy() => DoorTrigger.OnDoorTrigger -= OnDoorTriggerEventReceived;
        #endregion

        void Start() => IntializeDoor();

        void Update() => MoveAndRotateDoor();
        #endregion

        #region My Functions

        #region Enemy Door Control
        /// <summary>
        /// Function used from EnemyDoorTrigger to open Door when they enter the trigger;
        /// </summary>
        public void OpenDoorEnemy()
        {
            if (!_isOpened)
            {
                _isOpened = true;
                audDoor.PlayOneShot(sfxDoorOpen);
            }
        }

        /// <summary>
        /// Function used from EnemyDoorTrigger to close Door when they exit the trigger;
        /// </summary>
        public void CloseDoorEnemy()
        {
            if (_isOpened)
            {
                _isOpened = false;
                audDoor.PlayOneShot(sfxDoorClose);
            }
        }
        #endregion

        #region Door Controls
        /// <summary>
        /// Intialises the door. Spawns a closed door and disables it. It also disables the dopen door debug;
        /// </summary>
        void IntializeDoor()
        {
            _doorCloseObj = Instantiate(doorClose, doorClose.transform.position, doorClose.transform.rotation, transform);
            _doorCloseObj.SetActive(false);
            doorOpen.SetActive(false);
        }

        /// <summary>
        /// The door moves and rotates according to the enum chosen;
        /// </summary>
        void MoveAndRotateDoor()
        {
            if (!_isOnHold)
            {
                var target = _isOpened ? doorOpen : _doorCloseObj;

                if (currType == DoorType.Both)
                    doorClose.transform.SetPositionAndRotation(Vector3.MoveTowards(doorClose.transform.position, target.transform.position, moveSpeed * Time.deltaTime),
                        Quaternion.RotateTowards(doorClose.transform.rotation, target.transform.rotation, rotationSpeed * Time.deltaTime));

                if (currType == DoorType.Rotating)
                    doorClose.transform.rotation = Quaternion.RotateTowards(doorClose.transform.rotation, target.transform.rotation, rotationSpeed * Time.deltaTime);

                if (currType == DoorType.Moving)
                    doorClose.transform.position = Vector3.MoveTowards(doorClose.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            }
        }
        #endregion

        public bool IsDoorOpened()
        {
            if (_isOpened)
                return true;
            else
                return false;
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from DoorTrigger Script. This event just checks which door to control according to what trigger ID the player passed from;
        /// </summary>
        /// <param name="id"> ID from the trigger script must be the same as this script; </param>
        void OnDoorTriggerEventReceived(int id)
        {
            if (id == this.triggerID)
            {
                _isOpened = !_isOpened;

                if (sfxDoorOpen != null || sfxDoorClose != null)
                {
                    if (_isOpened)
                        audDoor.PlayOneShot(sfxDoorOpen);
                    else
                        audDoor.PlayOneShot(sfxDoorClose);
                }
            }
        }
        #endregion
    }
}
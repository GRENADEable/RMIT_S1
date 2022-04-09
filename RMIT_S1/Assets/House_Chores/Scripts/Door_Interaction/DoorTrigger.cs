using UnityEngine;

namespace Khatim
{
    public class DoorTrigger : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Enums")]
        [SerializeField]
        private DoorType currDoorType = DoorType.Door;

        private enum DoorType { Other, Door, Locker, Vent };

        [SerializeField]
        [Tooltip("Use iD numbers above zero as zero is default number. This is used to distinguish which door it will send events to")]
        private int triggerID = default;

        [Space, Header("Keys")]
        public bool isAccessable = default;

        [SerializeField]
        [Tooltip("Can this door use a key?")]
        private bool canUseKey = default;

        [SerializeField]
        [Tooltip("Remove key after use?")]
        private bool oneTimeOnly = default;

        [SerializeField]
        [Tooltip("The key this door accepts. Has to be the same exact KeyID as the keyItem Script or else it won't work")]
        private int keyTypeID = default;

        #region Events
        public delegate void SendEventsInt(int index);
        /// <summary>
        /// Event sent from DoorTrigger Script to DoorController Script;
        /// This just passes an int iD so that specific door with the same iD is opened;
        /// </summary>
        public static event SendEventsInt OnDoorTrigger;

        public delegate void SendEvents();
        /// <summary>
        /// Event sent from DoorTrigger Script to PropHolder Script;
        /// </summary>
        public static event SendEvents OnKeyUsed;
        #endregion

        #endregion

        #region My Functions
        /// <summary>
        /// Function that interacts with the door;
        /// </summary>
        /// <param name="playerObj"> Needs a GameObject in order to distinguish which door to open; </param>
        public void InteractDoor(GameObject playerObj)
        {
            if (isAccessable)
            {
                if (canUseKey)
                    CompareKey(playerObj);
                else
                    OnDoorTrigger?.Invoke(triggerID);
            }
        }

        /// <summary>
        /// Gets the key tpye integer;
        /// </summary>
        /// <returns> Key Integer; </returns>
        public int GetKeyTypeInt() => keyTypeID;

        /// <summary>
        /// Checks if they have the right key ID;
        /// </summary>
        void CompareKey(GameObject playerObj)
        {
            DoorInteraction key = playerObj.GetComponent<DoorInteraction>();

            if (key.ContainKeyInt(GetKeyTypeInt()))
            {
                if (oneTimeOnly)
                {
                    OnDoorTrigger?.Invoke(triggerID);
                    key.RemoveKeyInt(GetKeyTypeInt());
                    canUseKey = false;
                    oneTimeOnly = false;
                    OnKeyUsed?.Invoke();

                    if (currDoorType == DoorType.Vent)
                        isAccessable = false;
                }
                else
                    OnDoorTrigger?.Invoke(triggerID);
            }
            //else
            //    Debug.Log("Find Key");
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to Event from GameManager Script;
        /// Sets all the doors interactable;
        /// </summary>
        /// <param name="isInteractable"> If true, door can be interacted. If false, door can't be interacted; </param>
        void OnDoorInteractableEventReceived(bool isInteractable)
        {
            if (currDoorType == DoorType.Door)
                isAccessable = true;
        }
        #endregion
    }
}
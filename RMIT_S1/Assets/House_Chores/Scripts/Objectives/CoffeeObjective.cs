using UnityEngine;

namespace Khatim
{
    public class CoffeeObjective : MonoBehaviour
    {
        #region Serialized Variables

        #region Events
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from CoffeeObjective to GameManager;
        /// Changes to the 2nd Objective;
        /// </summary>
        public static event SendEvents OnShowObj2;

        /// <summary>
        /// Event sent from CoffeeObjective to GameManager;
        /// Changes to the 3rd Objective;
        /// </summary>
        public static event SendEvents OnShowObj3;
        #endregion

        #region Coffee Objs
        [SerializeField]
        [Tooltip("Current GameObject")]
        private CurrObjective _currCoffeeObj = CurrObjective.Wrong;
        private enum CurrObjective { Right, Wrong };

        [Tooltip("Coffee GameObjects")]
        [SerializeField] private GameObject coffeeRight = default, coffeeWrong = default;
        #endregion

        #endregion

        #region Private Variables
        private Animator _coffeeAnim = default;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            _coffeeAnim = GetComponent<Animator>();
        }
        #endregion

        #region My Functions
        public void PlayCoffeeAnim()
        {
            if (_currCoffeeObj == CurrObjective.Right)
                _coffeeAnim.Play("Coffee_Pour_Right_Anim");
            else
                _coffeeAnim.Play("Coffee_Pour_Wrong_Anim");
        }

        public void SwitchRightCoffee()
        {
            _currCoffeeObj = CurrObjective.Right;
            coffeeWrong.SetActive(false);
            coffeeRight.SetActive(true);
        }

        public void SwitchWrongCoffee()
        {
            _currCoffeeObj = CurrObjective.Wrong;
            coffeeWrong.SetActive(true);
            coffeeRight.SetActive(false);
        }
        #endregion

        #region Events
        public void ShowObj2() => OnShowObj2?.Invoke();

        public void ShowObj3() => OnShowObj3?.Invoke();
        #endregion
    }
}
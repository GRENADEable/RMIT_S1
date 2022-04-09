using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Khatim
{
    public class EnemyFSM : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Player Root GameObject")]
        private GameObject playerRoot = default;

        #region Enemy Variables
        [SerializeField]
        [Tooltip("Enemy Field of View")]
        private float enemyFov = default;

        [SerializeField]
        [Tooltip("Enemy idle Time duration")]
        private float idleTime = default;

        #region Wander
        [Space, Header("Enemy Wander Variables")]
        [SerializeField]
        [Tooltip("Radius the enemy can Wander")]
        private float wanderRadius = default;

        [SerializeField]
        [Tooltip("Maximum time the Enemy can Wander")]
        private float maxWanderTime = default;
        #endregion

        #region Enemy Speeds
        [Space, Header("Enemy Speeds")]
        [SerializeField]
        [Tooltip("Enemy wander Speed")]
        private float enemyWanderSpeed = default;

        [SerializeField]
        [Tooltip("Enemy chase Speed")]
        private float enemyChaseSpeed = default;

        [SerializeField]
        [Tooltip("Enemy seek Speed")]
        private float enemySeekSpeed = default;

        [SerializeField]
        [Tooltip("Enemy chase Speed on Final Level")]
        private float enemyChaseSpeedEscape = default;
        #endregion

        #region Distance Checks
        [Space, Header("Distance Checks")]
        [SerializeField]
        [Tooltip("What distance the Enemy Chases?")]
        private float playerCloseDistance = default;

        [SerializeField]
        [Tooltip("What distance the Enemy Chases?")]
        private float chaseDistance = default;

        [SerializeField]
        [Tooltip("What distance the Enemy Attacks?")]
        private float attackDistance = default;

        //[SerializeField]
        //[Tooltip("Distance when to stop seeking when you reach the seek Position")]
        //private float seekStopDistance = default;
        #endregion

        #region Player Check Raycast
        [Space, Header("Player Check Raycast")]
        [SerializeField]
        [Tooltip("Ray distance for Enemy")]
        private float rayDistance = default;

        [SerializeField]
        [Tooltip("Radius of the sphere at the end of the Ray")]
        private float raySphereRadius;

        [SerializeField]
        [Tooltip("Height of the Ray")]
        private Vector3 rayHeight = default;

        [SerializeField]
        [Tooltip("Height of the Ray when Chasing")]
        private Vector3 rayHeightChase = default;
        #endregion

        #endregion

        #region Events
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from EnemyFSM Script to GameManager Script;
        /// Event just kills the Player with a delay;
        /// </summary>
        public static event SendEvents OnPlayerDeath;

        public static event SendEvents OnPlayerChaseStarted;
        public static event SendEvents OnPlayerChaseEnded;
        #endregion

        #endregion

        #region Private Variables

        #region Enemy Variables
        [Header("Enemy Variables")]
        private NavMeshAgent _enemyAgent = default;
        [SerializeField]
        private EnemyState _currCondition = EnemyState.Wander;
        private enum EnemyState { Idle, Wander, ChasePlayer, SeekNoise, AttackPlayer };
        #endregion

        #region Distance Checks
        [Header("Distance Checks")]
        [SerializeField] private float _distanceToPlayer = default;
        [SerializeField] private float _currFov = default;
        private Vector3 _tarDir = default;
        private float _timer = default;
        #endregion

        #region Player Check Raycast
        [Header("Player Check Raycast")]
        private bool _isHitting = default;
        private bool _isChaseHitting = default;
        [SerializeField] private bool _isPlayerHiding = default;
        [SerializeField] private bool _isHittingPlayer = default;
        [SerializeField] private bool _isChaseHittingPlayer = default;
        private Ray _ray = default;
        private int playerLayer = default;
        private RaycastHit _hit = default;
        private RaycastHit _hitChase = default;
        #endregion

        #region Audio
        private bool _isChaseAudStartEventSent = default;
        private bool _isChaseAudEndEventSent = default;
        #endregion

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            HidingMechanic.OnPlayerHide += OnPlayerHideEventReceived;
            HideInBedCam.OnPlayerHide += OnPlayerHideEventReceived;
        }

        void OnDisable()
        {
            HidingMechanic.OnPlayerHide -= OnPlayerHideEventReceived;
            HideInBedCam.OnPlayerHide -= OnPlayerHideEventReceived;

            ChasePlayerEndAud();
        }

        void OnDestroy()
        {
            HidingMechanic.OnPlayerHide -= OnPlayerHideEventReceived;
            HideInBedCam.OnPlayerHide -= OnPlayerHideEventReceived;

            ChasePlayerEndAud();
        }
        #endregion

        void Start()
        {
            _enemyAgent = GetComponent<NavMeshAgent>();
            _timer = maxWanderTime;
            playerLayer = LayerMask.GetMask("Player");
        }

        void Update()
        {
            if (playerRoot.activeInHierarchy)
            {
                _distanceToPlayer = Vector3.Distance(transform.position, playerRoot.transform.position);
                //Debug.DrawRay(transform.position, transform.forward * chaseDistance, Color.green);
                //Debug.DrawRay(transform.position, transform.forward * attackDistance, Color.red);

                _tarDir = playerRoot.transform.position - transform.position;
                _currFov = Vector3.Angle(_tarDir, transform.forward);

                EnemyStates();
                PlayerCheck();
                CahseCheck();
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, wanderRadius);
        }
        #endregion

        #region My Functions

        #region Enemy FSM
        /// <summary>
        /// Using Switch and Case for Enemy AI;
        /// </summary>
        void EnemyStates()
        {
            switch (_currCondition)
            {
                case EnemyState.Idle:
                    break;

                case EnemyState.Wander:
                    Wander();

                    if ((_distanceToPlayer <= chaseDistance && _currFov < enemyFov || _distanceToPlayer < playerCloseDistance) && !_isPlayerHiding && _isChaseHittingPlayer)
                        _currCondition = EnemyState.ChasePlayer;

                    ChasePlayerEndAud();

                    break;

                case EnemyState.ChasePlayer:
                    Chase();

                    if (_distanceToPlayer >= chaseDistance || _isPlayerHiding || !_isChaseHittingPlayer)
                        _currCondition = EnemyState.Wander;

                    if (_distanceToPlayer <= attackDistance)
                        _currCondition = EnemyState.AttackPlayer;

                    ChasePlayerStartAud();

                    break;

                case EnemyState.SeekNoise:
                    SeekingPlayer();

                    if ((_distanceToPlayer <= chaseDistance && _currFov < enemyFov || _distanceToPlayer < playerCloseDistance) && _isChaseHittingPlayer)
                        _currCondition = EnemyState.ChasePlayer;

                    if (_isPlayerHiding)
                        _currCondition = EnemyState.Wander;

                    break;

                case EnemyState.AttackPlayer:

                    if (_isHittingPlayer)
                        Attack();

                    if (_distanceToPlayer >= attackDistance)
                        _currCondition = EnemyState.ChasePlayer;

                    if (_isPlayerHiding)
                        _currCondition = EnemyState.Wander;

                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Wander
        /// <summary>
        /// Enemy Wandering with a timer;
        /// </summary>
        void Wander()
        {
            _timer += Time.deltaTime;
            if (_timer >= maxWanderTime)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                _enemyAgent.SetDestination(newPos);
                _timer = 0;
            }

            _enemyAgent.speed = enemyWanderSpeed;
            //Debug.Log("Wandering");
        }

        /// <summary>
        /// This chooses a random point in the wander radius;
        /// </summary>
        /// <returns> Returns position withint the Navmesh Sphere; </returns>
        static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            //Sets a random position inside the sphere and that is multiplied with the distance and the center of the sphere.
            Vector3 randomPos = Random.insideUnitSphere * dist;

            //Vector 3 position is returned to the origin parameter.
            randomPos += origin;

            //Bool check if the random position is suitable on the navmesh. If true, then return the hit position.
            NavMesh.SamplePosition(randomPos, out NavMeshHit hit, dist, layermask);
            return hit.position;
        }
        #endregion

        #region Chase
        /// <summary>
        /// Chases Player with increased Speed;
        /// </summary>
        void Chase()
        {
            _enemyAgent.SetDestination(playerRoot.transform.position);
            _enemyAgent.speed = enemyChaseSpeed;
            //Debug.Log("Chasing Player");
        }

        void CahseCheck()
        {
            _isChaseHitting = Physics.Raycast(transform.position + rayHeightChase, (playerRoot.transform.position + rayHeight) - (transform.position + rayHeight), out _hitChase);
            Debug.DrawRay(transform.position + rayHeightChase, (playerRoot.transform.position + rayHeight) - (transform.position + rayHeight), Color.yellow);

            if (_isChaseHitting)
            {
                if (playerLayer == (playerLayer | (1 << _hitChase.collider.gameObject.layer)))
                    _isChaseHittingPlayer = true;
                else
                    _isChaseHittingPlayer = false;
            }
        }
        #endregion

        #region Seek Noise
        /// <summary>
        /// Chases the player when they make noise;
        /// </summary>
        void SeekingPlayer()
        {
            _enemyAgent.speed = enemySeekSpeed;
            _enemyAgent.SetDestination(playerRoot.transform.position);
            //Debug.Log("Seeking Player");
        }
        #endregion

        #region Attack
        /// <summary>
        /// Checks if the raycast is hitting directly at the player and not through walls;
        /// </summary>
        void PlayerCheck()
        {
            _ray = new Ray(transform.position + rayHeight, transform.forward);

            //_isHitting = Physics.Raycast(_ray, out _hit, rayDistance);
            _isHitting = Physics.SphereCast(_ray, raySphereRadius, out _hit, rayDistance);
            //Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, _isHitting ? Color.red : Color.white);

            if (_isHitting)
            {
                if (playerLayer == (playerLayer | (1 << _hit.collider.gameObject.layer)))
                    _isHittingPlayer = true;
                else
                    _isHittingPlayer = false;
            }
        }

        /// <summary>
        /// Attacks Player. One-hit kills them;
        /// </summary>
        void Attack()
        {
            StartCoroutine(IdleDelay());
            OnPlayerDeath?.Invoke();
            ChasePlayerEndAud();
            //Debug.Log("Attacking Player");
        }
        #endregion

        #region Audio
        void ChasePlayerStartAud()
        {
            if (!_isChaseAudStartEventSent)
            {
                _isChaseAudStartEventSent = true;
                _isChaseAudEndEventSent = false;
                OnPlayerChaseStarted?.Invoke();
                //Debug.Log("Chase Audio Started");
            }
        }

        void ChasePlayerEndAud()
        {
            if (!_isChaseAudEndEventSent)
            {
                _isChaseAudEndEventSent = true;
                _isChaseAudStartEventSent = false;
                OnPlayerChaseEnded?.Invoke();
                //Debug.Log("Chase Audio Ended");
            }
        }
        #endregion

        #endregion

        #region Coroutines
        IEnumerator IdleDelay()
        {
            _currCondition = EnemyState.Idle;
            _enemyAgent.enabled = false;
            yield return new WaitForSeconds(idleTime);
            _enemyAgent.enabled = true;
            _currCondition = EnemyState.Wander;
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from FPSControllerScript;
        /// Receives Vector3 of the last position the player was so that the Enemy can Seek;
        /// </summary>
        /// <param name="pos"></param>
        void OnPlayerDetectedEventReceived() => _currCondition = EnemyState.SeekNoise;

        /// <summary>
        /// Subbed to event from HidingMechanic Script;
        /// Checks if the player is hiding or not;
        /// </summary>
        /// <param name="isHiding"> If true, Enemy won't chase, if false, AI will chase; </param>
        void OnPlayerHideEventReceived(bool isHiding)
        {
            if (isHiding)
                _isPlayerHiding = true;
            else
                _isPlayerHiding = false;
        }

        /// <summary>
        /// Subbed to event from GameManager Script;
        /// Changes the Enemy chase speed to the new one;
        /// </summary>
        void OnPlayerEndChaseEventReceived() => enemyChaseSpeed = enemyChaseSpeedEscape;

        /// <summary>
        /// Subbed to event from GameManager Script;
        /// Changes the Enemy State to Idle;
        /// </summary>
        void OnGameEndEventReceived() => _currCondition = EnemyState.Idle;
        #endregion
    }
}
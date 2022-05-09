using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    #region Serialized Variables
    //[SerializeField]
    //[Tooltip("")]
    #endregion

    #region Private Variables
    [SerializeField] private ObstacleType _currObstacleType = ObstacleType.None;
    private enum ObstacleType { DisabledJump, SwitchedControls, None };
    [SerializeField] private bool _isObstacleEventSent = default;
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

    }

    void Update()
    {
        switch (_currObstacleType)
        {
            case ObstacleType.DisabledJump:
                break;

            case ObstacleType.SwitchedControls:
                break;

            case ObstacleType.None:
                break;

            default:
                break;
        }
    }
    #endregion

    #region My Functions

    #endregion

    #region Coroutines

    #endregion

    #region Events

    #endregion
}
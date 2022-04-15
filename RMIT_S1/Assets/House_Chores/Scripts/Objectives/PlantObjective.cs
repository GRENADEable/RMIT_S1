using UnityEngine;

public class PlantObjective : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Ivy GameObject")]
    private GameObject ivyPlant = default;

    #region Events
    public delegate void SendEvents();
    /// <summary>
    /// Event sent from PlantObjective to GameManager Script;
    /// Changes to the 8th Objective;
    /// </summary>
    public static event SendEvents OnShowObj8;
    #endregion

    #endregion

    #region My Functions
    public void OnIvyGrow()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        ivyPlant.SetActive(true);
        OnShowObj8?.Invoke();
    }
    #endregion
}
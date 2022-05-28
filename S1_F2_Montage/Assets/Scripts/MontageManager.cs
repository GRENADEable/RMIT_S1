using UnityEngine;
using UnityEngine.Playables;

public class MontageManager : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Mat Skybox")]
    private Material matSkybox = default;

    [SerializeField]
    [Tooltip("Montage Timeline")]
    private PlayableDirector montageTimeline = default;

    [SerializeField]
    [Tooltip("Crowd Manager Script")]
    private CrowdManager crwdManage = default;
    #endregion

    #region Private Variables
    [SerializeField] private bool _isTimelinePlayed = default;
    #endregion

    #region Unity Callbacks
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isTimelinePlayed)
        {
            _isTimelinePlayed = true;
            montageTimeline.Play();
            Debug.Log("Playing Montage");
        }
    }
    #endregion

    #region My Functions
    /// <summary>
    /// Subbed to Timeline event;
    /// Changes skybox;
    /// </summary>
    public void OnSkyboxChange() => RenderSettings.skybox = matSkybox;

    /// <summary>
    /// Subbed to Timeline event;
    /// Enables crowd control;
    /// </summary>
    public void OnCrowdAlive() => crwdManage.enabled = true;
    #endregion
}
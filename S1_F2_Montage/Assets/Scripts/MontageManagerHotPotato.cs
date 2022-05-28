using UnityEngine;
using UnityEngine.Playables;

public class MontageManagerHotPotato : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Mat Skybox")]
    private Material matSkybox = default;

    [SerializeField]
    [Tooltip("Montage Timeline")]
    private PlayableDirector montageTimeline = default;
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
    #endregion
}
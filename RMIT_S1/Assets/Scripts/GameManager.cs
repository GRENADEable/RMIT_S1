using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Fade Image Animation Component")]
    private Animator fadeBG = default;
    #endregion

    #region Unity Callbacks
    void Start() => fadeBG.Play("Fade_In");
    #endregion
}
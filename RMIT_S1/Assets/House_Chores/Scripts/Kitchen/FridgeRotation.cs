using UnityEngine;

public class FridgeRotation : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Rotation Speed")]
    private float rotationSpeed = default;
    #endregion

    #region Private Variables
    private bool _isRotating = default;
    #endregion

    #region Unity Callbacks
    void Update()
    {
        if (_isRotating)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _isRotating = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _isRotating = false;
    }
    #endregion
}
using UnityEngine;

public class SpotlightRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second.")]
    public float rotationSpeed = 30f;

    [Tooltip("Rotation axis (e.g., Y-axis for horizontal sweep).")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Rotate around the chosen axis
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime);
    }
}

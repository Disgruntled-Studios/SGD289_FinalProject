using UnityEngine;

public class InfiniteRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up; // Default axis: Y-axis
    public float rotationSpeed = 45f; // Degrees per second

    void Update()
    {
        transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.Self);
    }
}

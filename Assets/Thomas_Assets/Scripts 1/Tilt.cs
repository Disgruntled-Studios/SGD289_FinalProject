using UnityEngine;

public class OscillatingTilt : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }
    public RotationAxis rotationAxis = RotationAxis.X;

    [Tooltip("Minimum angle the object will tilt to")]
    public float minAngle = -30f;

    [Tooltip("Maximum angle the object will tilt to")]
    public float maxAngle = 30f;

    [Tooltip("Speed at which the object tilts back and forth")]
    public float speed = 1f;

    private float angle;

    void Update()
    {
        // Time-based interpolation using sine wave (output range: -1 to 1)
        float t = Mathf.Sin(Time.time * speed) * 0.5f + 0.5f; // Normalized to 0-1
        angle = Mathf.Lerp(minAngle, maxAngle, t);

        Vector3 rotation = Vector3.zero;
        switch (rotationAxis)
        {
            case RotationAxis.X:
                rotation = new Vector3(angle, 0f, 0f);
                break;
            case RotationAxis.Y:
                rotation = new Vector3(0f, angle, 0f);
                break;
            case RotationAxis.Z:
                rotation = new Vector3(0f, 0f, angle);
                break;
        }

        transform.localRotation = Quaternion.Euler(rotation);
    }
}


using UnityEngine;

public class InfiniteRotation : MonoBehaviour
{
    // Rotation speed in degrees per second
    public Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

    void Update()
    {
        // Rotate the object smoothly over time
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}

using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 360f;

    private void Update()
    {
        var deltaRotation = _rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, deltaRotation, Space.Self);
    }
}

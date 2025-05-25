using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float _movementSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Transform _cameraTransform;

    private Rigidbody _rb;
    private Vector2 _inputVector;
    
    public void OnMove(InputAction.CallbackContext context) => _inputVector = context.ReadValue<Vector2>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovementAndRotation();
    }

    private void HandleMovementAndRotation()
    {
        var camForward = _cameraTransform.forward;
        var camRight = _cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Convert 2D input into a 3D direction relative to the camera
        var moveDirection = camRight * _inputVector.x + camForward * _inputVector.y;

        // Move the player
        var newVelocity = new Vector3(moveDirection.x * _movementSpeed, _rb.linearVelocity.y, moveDirection.z * _movementSpeed);
        _rb.linearVelocity = newVelocity;

        // Rotate toward movement direction
        if (!(moveDirection.sqrMagnitude > 0.001f)) return;
        
        var targetRotation = Quaternion.LookRotation(moveDirection);
        _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FPSWorldTrigger"))
        {
            Debug.Log("FPS");
        }

        if (other.CompareTag("PlatformWorldTrigger"))
        {
            Debug.Log("Platform");
        }

        if (other.CompareTag("StealthWorldTrigger"))
        {
            Debug.Log("Stealth");
        }
    }
}


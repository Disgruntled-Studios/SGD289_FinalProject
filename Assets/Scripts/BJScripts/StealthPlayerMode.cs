using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StealthPlayerMode : IPlayerMode
{
    private readonly float _speed;
    private readonly float _rotationSpeed;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private bool _isCrouching;

    public StealthPlayerMode(float speed, float rotationSpeed, Transform playerTransform)
    {
        _speed = speed;
        _rotationSpeed = rotationSpeed;
        _playerTransform = playerTransform;
    }
    
    public StealthPlayerMode(float speed, float rotationSpeed, Transform playerTransform, Transform cameraTransform)
    {
        _speed = speed;
        _rotationSpeed = rotationSpeed;
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
    }

    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        var currentSpeed = _isCrouching ? _speed * 0.5f : _speed;

        // No movement? Exit early
        if (input == Vector2.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        var cameraTransform = Camera.main!.transform;

        var camForward = cameraTransform.forward;
        var camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        
        camForward.Normalize();
        camRight.Normalize();

        // Move based on camera's orientation
        var moveDir = (camForward * input.y + camRight * input.x).normalized;

        // Apply movement
        var velocity = moveDir * currentSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        
        // Rotate to face direction of movement
        var targetRotation = Quaternion.LookRotation(moveDir);
        context.rotation = Quaternion.Slerp(context.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }

    public void Rotate(Vector2 input)
    {
        const float sensitivity = 2f;
        _playerTransform.Rotate(Vector3.up, input.x * sensitivity);
    }

    public void Jump() { } // Not used in stealth

    public void Crouch(bool isPressed)
    {
        _isCrouching = isPressed;
    }

    public void Tick() { } // Not used in stealth
    public void Aim(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }
}

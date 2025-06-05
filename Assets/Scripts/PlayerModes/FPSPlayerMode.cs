using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSPlayerMode : IPlayerMode
{
    private readonly float _speed;
    private readonly float _rotationSpeed;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraPivot;

    private float _xRotation;
    private const float ClampAngle = 50f;
    private bool _isCrouching;
    private readonly FPSGunController _fpsGunController;

    public FPSPlayerMode(float speed, float rotationSpeed, Transform playerTransform, Transform cameraPivot, FPSGunController gunController)
    {
        _speed = speed;
        _rotationSpeed = rotationSpeed;
        _playerTransform = playerTransform;
        _cameraPivot = cameraPivot;
        _fpsGunController = gunController;
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

        var camForward = _cameraPivot.forward;
        var camRight = _cameraPivot.right;

        camForward.y = 0;
        camRight.y = 0;
        
        camForward.Normalize();
        camRight.Normalize();

        // Move based on camera's orientation
        var moveDir = (camForward * input.y + camRight * input.x).normalized;

        // Apply movement
        var velocity = moveDir * currentSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    public void Rotate(Vector2 input, Transform context)
    {
        const float sensitivity = 0.25f;

        _playerTransform.Rotate(Vector3.up, input.x * sensitivity);

        _xRotation -= input.y * sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -ClampAngle, ClampAngle);

        _cameraPivot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    public void Jump()
    {
        return;
    } // Not used in stealth

    public void Crouch(bool isPressed)
    {
        _isCrouching = isPressed;
    }

    public void Tick()
    {
        return;
    } 
    
    public void Aim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _fpsGunController.StartGunAim();
        }
        
        if (context.canceled)
        {
            _fpsGunController.EndGunAim();
        }
    }

    public void Attack()
    {
        _fpsGunController.ShouldShoot = true;
    }
}

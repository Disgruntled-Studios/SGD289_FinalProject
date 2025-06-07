using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSPlayerMode : IPlayerMode
{
    private readonly float _speed;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraPivot;

    private float _xRotation;
    private const float ClampAngle = 50f;
    private bool _isCrouching;
    private readonly FPSGunController _fpsGunController;

    private readonly bool _isBulletTime;

    public FPSPlayerMode(float speed, Transform playerTransform, Transform cameraPivot, FPSGunController gunController, bool isBulletTime)
    {
        _speed = speed;
        _playerTransform = playerTransform;
        _cameraPivot = cameraPivot;
        _fpsGunController = gunController;
        _isBulletTime = isBulletTime;
    }

    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        var currentSpeed = _isCrouching ? _speed * 0.5f : _speed;

        if (input == Vector2.zero) return;

        var camForward = _cameraPivot.forward;
        var camRight = _cameraPivot.right;

        camForward.y = 0;
        camRight.y = 0;
        
        camForward.Normalize();
        camRight.Normalize();

        var moveDirection = (camForward * input.y + camRight * input.x).normalized;

        Vector3 targetPosition;
        
        if (_isBulletTime)
        {
            targetPosition = rb.position + moveDirection * (currentSpeed * Time.fixedUnscaledDeltaTime);
        }
        else
        {
            targetPosition = rb.position + moveDirection * (currentSpeed * Time.fixedDeltaTime);
        }

        rb.MovePosition(targetPosition);
    }

    public void Rotate(Vector2 input, Transform context)
    {
        const float sensitivity = 0.33f;

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
        _fpsGunController.Shoot();
    }

    public void Special()
    {
        _fpsGunController.ChangeColor();
    }
}

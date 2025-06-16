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
    private readonly Rigidbody _playerRb;
    private readonly bool _isBulletTime;
    private readonly CapsuleCollider _standingCollider;
    private readonly SphereCollider _crouchingCollider;

    private const float JumpForce = 7f;
    private bool _isSprinting;

    private float _currentStamina;
    private const float MaxStamina = 100f;
    private const float StaminaDrainRate = 15f; // Per second
    private const float StaminaRegenRate = 20; // Per second
    private const float SprintMultiplier = 1.5f;
    private bool CanSprint => _currentStamina > 0f;

    private PlayerAnimationController _animationController;
    private readonly LayerMask _groundLayer;

    private const float GroundCheckRadius = 0.2f;
    private readonly Vector3 _groundCheckOffset = new(0, -0.9f, 0);

    public FPSPlayerMode(float speed, Transform playerTransform, Transform cameraPivot, FPSGunController gunController, bool isBulletTime, Rigidbody playerRb, CapsuleCollider standingCollider, SphereCollider crouchingCollider, PlayerAnimationController animationController, LayerMask groundLayer)
    {
        _speed = speed;
        _playerTransform = playerTransform;
        _cameraPivot = cameraPivot;
        _fpsGunController = gunController;
        _isBulletTime = isBulletTime;
        _playerRb = playerRb;
        _standingCollider = standingCollider;
        _crouchingCollider = crouchingCollider;
        _currentStamina = MaxStamina;
        _animationController = animationController;
        _groundLayer = groundLayer;
    }

    private bool IsGrounded()
    {
        var checkPosition = _playerTransform.position + _groundCheckOffset;
        var grounded = Physics.CheckSphere(checkPosition, GroundCheckRadius, _groundLayer, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(checkPosition, Vector3.up * 0.1f, grounded ? Color.green : Color.red, 0.1f);
        return grounded;
    }

    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        var baseSpeed = _speed;

        if (_isCrouching)
        {
            baseSpeed *= 0.5f;
        }

        if (_isSprinting && CanSprint)
        {
            baseSpeed *= SprintMultiplier;
        }

        if (input == Vector2.zero) return;

        var camForward = _cameraPivot.forward;
        var camRight = _cameraPivot.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        var moveDirection = (camForward * input.y + camRight * input.x).normalized;

        var deltaTime = _isBulletTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
        var targetPosition = rb.position + moveDirection * (baseSpeed * deltaTime);

        rb.MovePosition(targetPosition);
    }

    public void Rotate(Vector2 input, Transform context)
    {
        // Higher = faster, lower = slower
        const float mouseSensitivity = 0.4f;
        const float controllerSensitivity = 0.6f;

        float sensitivity;
        if (InputManager.Instance.IsUsingKeyboard)
        {
            sensitivity = mouseSensitivity;
        }
        else if (InputManager.Instance.IsUsingController)
        {
            sensitivity = controllerSensitivity;
        }
        else
        {
            sensitivity = mouseSensitivity;
        }
        
        _playerTransform.Rotate(Vector3.up, input.x * sensitivity);

        _xRotation -= input.y * sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -ClampAngle, ClampAngle);

        _cameraPivot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            _playerRb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    } 

    public void Crouch(bool isPressed)
    {
        _isCrouching = isPressed;
    }

    public void Tick()
    {
        if (_isSprinting && CanSprint)
        {
            _currentStamina -= StaminaDrainRate * Time.deltaTime;
            if (_currentStamina <= 0f)
            {
                _currentStamina = 0f;
                _isSprinting = false;
            }
        }
        else
        {
            _currentStamina += StaminaRegenRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
        }

        FPSManager.Instance?.UI.UpdateStaminaBar(_currentStamina, MaxStamina);
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
    
    public void OnModeEnter()
    {
        _fpsGunController.ToggleGunAndHands(true);
        _fpsGunController.ToggleLaser(false);
    }
    
    public void OnModeExit()
    {
        _fpsGunController.ToggleGunAndHands(false);
    }
    
    public void Sprint(InputAction.CallbackContext context)
    {
        if (_isCrouching) return;
        
        if (context.started && CanSprint)
        {
            _isSprinting = true;
        }

        if (context.canceled)
        {
            _isSprinting = false;
        }
    }
}

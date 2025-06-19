using System;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerAnimationController _animationController;
    [SerializeField] private TankGunController _gunController;
    [SerializeField] private CapsuleCollider _standingCollider;
    [SerializeField] private SphereCollider _crouchCollider;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private PlayerInventory _inventory;

    [Header("Movement Settings")] 
    [SerializeField] private float _normalSpeed = 3f;
    [SerializeField] private float _halfSpeed = 1.5f;
    [SerializeField] private float _rotationSpeed = 150f;
    [SerializeField] private LayerMask _groundLayer;
    private const float PlayerHeight = 2f;

    private float _currentSpeed;
    private float _currentMoveInput;
    public float CurrentMoveInput => _currentMoveInput;
    private float _currentRotationInput;
    private float _currentRotationSpeed;

    private const float RotationAcceleration = 8f;
    private const float RotationDeceleration = 10f;
    private const float RotationDeadZone = 0.05f;
    private const float MoveResponsiveness = 10f;
    private const float StopResponsiveness = 15f;

    private bool _isCrouching;
    public bool IsCrouching => _isCrouching;
    private bool _isGrounded;
    private bool _forcedInitialVelocityApplied;

    private IInteractable _currentInteractable;
    
    private void Awake()
    {
        _currentSpeed = _normalSpeed;
        _currentRotationSpeed = _rotationSpeed;
        _crouchCollider.enabled = false;
        _laser.enabled = false;
    }

    private void FixedUpdate()
    {
        if (InputManager.Instance.IsInUI || InputManager.Instance.IsInPuzzle) return;

        CheckGrounded();
        ApplyRotation();
        ApplyMovement();
    }

    public void OnMove(InputAction.CallbackContext context) => _currentMoveInput = context.ReadValue<float>();

    public void OnRotate(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<float>();
        if (Mathf.Abs(input) > 0.01f)
        {
            _currentRotationInput =
                Mathf.MoveTowards(_currentRotationInput, input, Time.deltaTime * RotationAcceleration);
        }
        else
        {
            _currentRotationInput = Mathf.MoveTowards(_currentRotationInput, 0f, Time.deltaTime * RotationDeceleration);
        }

        if (Mathf.Abs(_currentRotationInput) < RotationDeadZone)
        {
            _currentRotationInput = 0f;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.performed || InputManager.Instance.IsInPuzzle) return;

        if (_standingCollider.enabled)
        {
            SetCrouchState(true);
        }
        else if (!Physics.Raycast(transform.TransformPoint(_crouchCollider.center), Vector3.up, 1f))
        {
            SetCrouchState(false);
        }

        UpdateSpeed();
    }

    private void SetCrouchState(bool isCrouching)
    {
        _isCrouching = isCrouching;
        _animationController.Crouch(_isCrouching);
        _standingCollider.enabled = !_isCrouching;
        _crouchCollider.enabled = _isCrouching;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.IsInPuzzle) return;

        if (context.started)
        {
            _gunController.StartGunAim();
            _animationController.Aim(true);
            _laser.enabled = true;
            _currentRotationSpeed = _rotationSpeed * 0.25f;
            UpdateSpeed();
        }

        if (context.canceled)
        {
            _gunController.EndGunAim();
            _animationController.Aim(false);
            _laser.enabled = false;
            _currentRotationSpeed = _rotationSpeed;
            UpdateSpeed();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !InputManager.Instance.IsInPuzzle)
        {
            _gunController.HandleShoot();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        return;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        return;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentInteractable?.Interact(transform, _inventory);
        }
    }

    public void OnSpecial(InputAction.CallbackContext context)
    {
        return;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.TogglePauseGame();
        }
    }

    private void ApplyRotation()
    {
        if (Mathf.Approximately(_currentRotationInput, 0f)) return;

        var rotationAmount = _currentRotationInput * _currentRotationSpeed * Time.deltaTime;
        var deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
    }

    private void ApplyMovement()
    {
        var targetVelocity = transform.forward * (_currentMoveInput * _currentSpeed);

        if (_currentMoveInput != 0f)
        {
            if (_rb.linearVelocity.magnitude < 0.01f && !_forcedInitialVelocityApplied)
            {
                _rb.linearVelocity = targetVelocity;
                _forcedInitialVelocityApplied = true;
            }
            else
            {
                _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, targetVelocity,
                    Time.deltaTime * MoveResponsiveness);
            }
        }
        else
        {
            _rb.linearVelocity =
                Vector3.MoveTowards(_rb.linearVelocity, Vector3.zero, Time.deltaTime * StopResponsiveness);
        }
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, _groundLayer);
    }

    private void UpdateSpeed()
    {
        _currentSpeed = (_gunController.isReloading || _isCrouching) ? _halfSpeed : _normalSpeed;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            _currentInteractable = interactable;
        }
        else if (other.transform.parent != null &&
                 other.transform.parent.TryGetComponent(out interactable))
        {
            _currentInteractable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_currentInteractable == null) return;
        
        if ((other.TryGetComponent<IInteractable>(out var interactable) || other.transform.parent.TryGetComponent(out interactable)) && interactable == _currentInteractable)
        {
            _currentInteractable?.OnExit();
            _currentInteractable = null;
        }
    }
}


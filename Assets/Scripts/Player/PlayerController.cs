using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerAnimationController _animationController;
    [SerializeField] private GunController _gunController;
    [SerializeField] private CapsuleCollider _standingCollider;
    [SerializeField] private SphereCollider _crouchCollider;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private PlayerInventory _inventory;
    private PlayerHealth _health;

    [Header("Movement Settings")] 
    [SerializeField] private float _normalSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationSmoothTime;
    [SerializeField] private LayerMask _groundLayer;
    private const float PlayerHeight = 2f;
    private const float SprintMultiplier = 2.5f;
    private const float CrouchMultiplier = 0.5f;
    private const float AimSpeedMultiplier = 0.75f;
    private const float AimRotationMultiplier = 0.25f;

    private float _currentSpeed;
    private float _currentMoveInput;
    public float CurrentMoveInput => _currentMoveInput;
    private float _currentRotationInput;
    private float _smoothedRotationInput;
    private float _currentRotationVelocity;
    private float _currentRotationSpeed;

    private const float AccelerationRate = 10f;
    private const float DecelerationRate = 15f;
    private const float GroundDrag = 2f;

    private bool _isCrouching;
    public bool IsCrouching => _isCrouching;
    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;
    private bool _isSprinting;
    public bool IsSprinting => _isSprinting;

    private IInteractable _currentInteractable;
    private IItemReceiver _currentItemReceiver;
    public IItemReceiver CurrentItemReceiver => _currentItemReceiver;
    
    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        _currentSpeed = _normalSpeed;
        _currentRotationSpeed = _rotationSpeed;
        _crouchCollider.enabled = false;
        _laser.enabled = false;
        
        _rb.WakeUp();
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.linearDamping = GroundDrag;
        
        Physics.SyncTransforms();
    }
    
    public float GetCurrentTurnInput() => _currentRotationInput;

    private void FixedUpdate()
    {
        if (InputManager.Instance.IsInUI || InputManager.Instance.IsInPuzzle || _health.Health.IsDead) return;

        CheckGrounded();
        ApplyRotation();
        ApplyMovement();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _currentMoveInput = context.ReadValue<float>();
    } 

    public void OnRotate(InputAction.CallbackContext context)
    {
        _currentRotationInput = context.ReadValue<float>();
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

        if (context.started && _gunController.hasItem)
        {
            _gunController.StartGunAim();
            _animationController.Aim(true);
            _laser.enabled = true;
            _currentRotationSpeed = _rotationSpeed * AimRotationMultiplier;
            UpdateSpeed();
        }

        if (context.canceled && _gunController.hasItem)
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
        if (InputManager.Instance.IsInPuzzle || _isCrouching) return;

        if (context.started)
        {
            _isSprinting = true;
            _animationController.Sprint(_isSprinting);
            UpdateSpeed();
        }

        if (context.canceled)
        {
            _isSprinting = false;
            _animationController.Sprint(_isSprinting);
            UpdateSpeed();
        }
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
        _smoothedRotationInput = Mathf.SmoothDamp(_smoothedRotationInput, _currentRotationInput,
            ref _currentRotationVelocity, _rotationSmoothTime);

        if (Mathf.Approximately(_smoothedRotationInput, 0f)) return;

        var rotationAmount = _smoothedRotationInput * _currentRotationSpeed * Time.deltaTime;
        var deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
    }

    private void ApplyMovement()
    {
        var targetVelocity = transform.forward * (_currentMoveInput * _currentSpeed);

        if (_currentMoveInput != 0f)
        {
            var acceleration = _isSprinting ? AccelerationRate * 1.25f : AccelerationRate;
            _rb.linearVelocity =
                Vector3.MoveTowards(_rb.linearVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            _rb.linearVelocity =
                Vector3.MoveTowards(_rb.linearVelocity, Vector3.zero, DecelerationRate * Time.deltaTime);
        }
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, _groundLayer);
    }

    private void UpdateSpeed()
    {
        if (_gunController.IsReloading || _isCrouching)
        {
            _currentSpeed = _normalSpeed * CrouchMultiplier;
        }
        else if (_gunController.IsAiming)
        {
            _currentSpeed = _normalSpeed * AimSpeedMultiplier;
        }
        else if (_isSprinting)
        {
            _currentSpeed = _normalSpeed * SprintMultiplier;
        }
        else
        {
            _currentSpeed = _normalSpeed;
        }
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

        if (other.TryGetComponent<IItemReceiver>(out var receiver))
        {
            _currentItemReceiver = receiver;
        }
        else if (other.transform.parent != null && other.transform.parent.TryGetComponent(out receiver))
        {
            _currentItemReceiver = receiver;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_currentInteractable != null)
        {
            var isAMatch = false;

            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                isAMatch = interactable == _currentInteractable;
            }
            else if (other.transform.parent != null && other.transform.parent.TryGetComponent(out interactable))
            {
                isAMatch = interactable == _currentInteractable;
            }

            if (isAMatch)
            {
                _currentInteractable.OnExit();
                ClearCurrentInteractable(_currentInteractable);
            }
        }

        if (_currentItemReceiver != null)
        {
            var isAMatch = false;

            if (other.TryGetComponent<IItemReceiver>(out var receiver))
            {
                isAMatch = receiver == _currentItemReceiver;
            }
            else if (other.transform.parent != null && other.transform.parent.TryGetComponent(out receiver))
            {
                isAMatch = receiver == _currentItemReceiver;
            }

            if (isAMatch)
            {
                ClearCurrentItemReceiver(_currentItemReceiver);
            }
        }
    }

    public void ClearCurrentInteractable(IInteractable interactable)
    {
        if (_currentInteractable == interactable)
        {
            _currentInteractable = null;
        }
    }

    public void ClearCurrentItemReceiver(IItemReceiver receiver)
    {
        if (_currentItemReceiver == receiver)
        {
            _currentItemReceiver = null;
        }
    }
}


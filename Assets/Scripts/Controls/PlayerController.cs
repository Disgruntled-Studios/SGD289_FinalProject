using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerAnimationController _animationController;
    [SerializeField] private GunController _gunController;
    public GunController GunController => _gunController;
    [SerializeField] private CapsuleCollider _standingCollider;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private PlayerInventory _inventory;
    private PlayerHealth _health;

    [Header("Movement Settings")]
    [SerializeField] private float _normalSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationSmoothTime;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _headCheck;
    [SerializeField] private LayerMask _overheadLayer;
    
    private const float CrouchHeight = 1.75f;
    private float _originalHeight;
    private Vector3 _originalCenter;
    [SerializeField] private SphereCollider _headCollider;
    
    private const float DefaultSpeedMultiplier = 1f;
    private const float SprintSpeedMultiplier = 1.75f;
    private const float CrouchSpeedMultiplier = 0.5f;
    private const float InjuredSpeedMultiplier = 0.75f;
    private const float BackwardsSpeedMultiplier = 0.75f;
    private const float AimSpeedMultiplier = 1f;

    private const float CrouchRotationMultiplier = 0.75f;
    private const float DefaultRotationMultiplier = 1.125f;
    private const float AimRotationMultiplier = 1.25f;
    private const float SprintRotationMultiplier = 1.25f;

    private float _currentSpeed;
    private float _currentMoveInput;
    public float CurrentMoveInput => _currentMoveInput;
    private float _currentRotationInput;
    private float _smoothedRotationInput;
    private float _currentRotationVelocity;
    private float _currentRotationSpeed;

    private const float GroundDrag = 2f;

    private Vector3 _movementVelocity;
    private Vector3 _currentVelocitySmoothDamp;

    private bool _isCrouching;
    public bool IsCrouching => _isCrouching;
    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;
    private bool _isSprinting;
    public bool IsSprinting => _isSprinting;

    public IInteractable _currentInteractable { get; private set; }
    private IItemReceiver _currentItemReceiver;
    public IItemReceiver CurrentItemReceiver => _currentItemReceiver;
    public Transform currentHighlightedObj;

    private PlayerInput Input => InputManager.Instance.PlayerInput;

    public bool IsMovementLocked { get; set; } = false;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        _currentSpeed = _normalSpeed;
        _currentRotationSpeed = _rotationSpeed;
        _laser.enabled = false;

        _rb.WakeUp();
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.linearDamping = GroundDrag;

        _originalHeight = _standingCollider.height;
        _originalCenter = _standingCollider.center;
        
        Physics.SyncTransforms();
    }

    private void OnEnable()
    {
        var playerMap = Input.PlayerMap;

        playerMap.Move.performed += OnMove;
        playerMap.Move.canceled += OnMove;
        playerMap.Rotate.performed += OnRotate;
        playerMap.Rotate.canceled += OnRotate;
        playerMap.Crouch.performed += OnCrouch;
        playerMap.Aim.started += OnAim;
        playerMap.Aim.canceled += OnAim;
        playerMap.Attack.performed += OnAttack;
        playerMap.Sprint.started += OnSprint;
        playerMap.Sprint.canceled += OnSprint;
        playerMap.Interact.performed += OnInteract;
        playerMap.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        var playerMap = Input.PlayerMap;

        playerMap.Move.performed -= OnMove;
        playerMap.Move.canceled -= OnMove;
        playerMap.Rotate.performed -= OnRotate;
        playerMap.Rotate.canceled -= OnRotate;
        playerMap.Crouch.performed -= OnCrouch;
        playerMap.Aim.started -= OnAim;
        playerMap.Aim.canceled -= OnAim;
        playerMap.Attack.performed -= OnAttack;
        playerMap.Sprint.started -= OnSprint;
        playerMap.Sprint.canceled -= OnSprint;
        playerMap.Interact.performed -= OnInteract;
        playerMap.Pause.performed -= OnPause;
    }

    public float GetCurrentTurnInput() => _currentRotationInput;
    
    private void FixedUpdate()
    {
        if (InputManager.Instance.IsInUI || InputManager.Instance.IsInPuzzle || _health.Health.IsDead || IsMovementLocked) return;

        if (_isSprinting && _currentMoveInput < 0.01f)
        {
            _isSprinting = false;
            _animationController.Sprint(false);
        }

        CheckGrounded();
        ApplyRotation();
        ApplyMovement();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsMovementLocked) return;
        
        _currentMoveInput = context.ReadValue<float>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (IsMovementLocked) return;
        
        _currentRotationInput = context.ReadValue<float>();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context) || _gunController.IsAiming || IsMovementLocked || _isSprinting) return;

        if (!_isCrouching)
        {
            SetCrouchState(true);
        }
        else if (CanUncrouch())
        {
            SetCrouchState(false);
        }

        UpdateSpeed();
    }

    private bool CanUncrouch()
    {
        if (!_standingCollider) return false;

        var standingHeight = _originalHeight;
        var currentHeight = _standingCollider.height;
        var heightDifference = standingHeight - currentHeight;

        var currentTop = transform.position.y + _standingCollider.center.y + (currentHeight * 0.5f);
        var rayOrigin = new Vector3(transform.position.x, currentTop, transform.position.z);

        return !Physics.Raycast(rayOrigin, Vector3.up, heightDifference, _overheadLayer);
    }

    private void SetCrouchState(bool isCrouching)
    {
        _isCrouching = isCrouching;
        _animationController.Crouch(_isCrouching);

        if (_headCollider)
        {
            _headCollider.enabled = _isCrouching;
        }
        
        if (!_standingCollider) return;

        var targetHeight = isCrouching ? CrouchHeight : _originalHeight;
        var heightDiff = _originalHeight - targetHeight;

        _standingCollider.height = targetHeight;

        var newCenter = _originalCenter;
        newCenter.y -= heightDiff * 0.5f;
        _standingCollider.center = newCenter;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (IsCrouching || IsMovementLocked) return;
        
        if (context.started && _gunController.HasGun)
        {
            _gunController.StartGunAim();
            _animationController.Aim(true);
            _laser.enabled = true;
            UpdateSpeed();
        }

        if (context.canceled && _gunController.HasGun)
        {
            _gunController.EndGunAim();
            _animationController.Aim(false);
            _laser.enabled = false;
            UpdateSpeed();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context) || IsMovementLocked) return;

        _gunController.HandleShoot();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context) || IsMovementLocked) return;

        return;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (IsMovementLocked || _isCrouching) return;
        
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
        if (InputManager.Instance.ShouldBlockInput(context) || IsMovementLocked) return;

        if (_currentInteractable != null)
        {
            _currentInteractable?.Interact(transform, _inventory);
            return;
        }

        if (_currentItemReceiver != null && !_currentItemReceiver.ItemHasBeenReceived)
        {
            var correctItem = _inventory.Items.FirstOrDefault(item =>
                item?.itemName == _currentItemReceiver.RequiredItemName && !item.isGun && !item.isNote);

            if (correctItem == null) return;
            if (_currentItemReceiver.TryReceiveItem(_inventory, correctItem))
            {
                UIManager.Instance.StartPopUpText($"{correctItem.itemName} used on {_currentItemReceiver.Name}");
            }
        }
    }

    public void OnSpecial(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context) || IsMovementLocked) return;

        // _gunController.StartCoroutine(_gunController.ReloadGun());
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context) || IsMovementLocked) return;

        UIManager.Instance.OpenPauseMenu();
    }

    private void ApplyRotation()
    {
        if (_currentMoveInput > 0.01f || _gunController.IsAiming)
        {
            if (Mathf.Approximately(_currentRotationInput, 0f)) return;
            
            var rotationAmount = _currentRotationInput * _currentRotationSpeed * Time.fixedDeltaTime;
            var deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
            _rb.MoveRotation(_rb.rotation * deltaRotation);
            return;
        }
        
        _smoothedRotationInput = Mathf.SmoothDamp(_smoothedRotationInput, _currentRotationInput,
            ref _currentRotationVelocity, _rotationSmoothTime);

        if (Mathf.Approximately(_smoothedRotationInput, 0f)) return;

        var idleRotationAmount = _smoothedRotationInput * _currentRotationSpeed * Time.fixedDeltaTime;
        var idleDeltaRotation = Quaternion.Euler(0f, idleRotationAmount, 0f);
        _rb.MoveRotation(_rb.rotation * idleDeltaRotation);
    }

    private void ApplyMovement()
    {
        var forwardBackwardMultiplier = _currentMoveInput < -0.01f ? BackwardsSpeedMultiplier : 1f;
        var forward = transform.forward * (_currentMoveInput * _currentSpeed * forwardBackwardMultiplier);

        _rb.linearVelocity = new Vector3(forward.x, _rb.linearVelocity.y, forward.z);
    }

    private void CheckGrounded()
    {
        if (!_standingCollider)
        {
            _isGrounded = false;
            return;
        }

        var height = _standingCollider.height;
        var radius = _standingCollider.radius;

        var checkDistance = (height * 0.5f) - radius + 0.2f;

        _isGrounded = Physics.Raycast(transform.position, Vector3.down, checkDistance, _groundLayer);
    }

    private void UpdateSpeed()
    {
        if (_isCrouching)
        {
            _currentSpeed = _normalSpeed * CrouchSpeedMultiplier;
            _currentRotationSpeed = _rotationSpeed * CrouchRotationMultiplier;
        }
        else if (_gunController.IsAiming)
        {
            _currentSpeed = _normalSpeed * AimSpeedMultiplier;
            _currentRotationSpeed = _rotationSpeed * AimRotationMultiplier;
        }
        else if (_isSprinting && _currentMoveInput > 0.01f)
        {
            _currentSpeed = _normalSpeed * SprintSpeedMultiplier;
            _currentRotationSpeed = _rotationSpeed * SprintRotationMultiplier;
        }
        else
        {
            _currentSpeed = _normalSpeed * DefaultSpeedMultiplier;
            _currentRotationSpeed = _rotationSpeed * DefaultRotationMultiplier;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var targetTransform = other.transform;
        if (!targetTransform.TryGetComponent<IInteractable>(out var interactable) && targetTransform.parent)
        {
            targetTransform = targetTransform.parent;
            targetTransform.TryGetComponent(out interactable);
        }

        if (interactable != null)
        {
            _currentInteractable = interactable;
            _currentInteractable?.OnEnter();

            if (targetTransform.TryGetComponent<PressureValveSystem>(out var doorPressureGame) &&
                doorPressureGame.HighlightableObj)
            {
                currentHighlightedObj = doorPressureGame.HighlightableObj;
            }
            else if (targetTransform.TryGetComponent<PowerPuzzleManager>(out var powerPuzzleManager) &&
                     powerPuzzleManager.HighlightableObj)
            {
                //currentHighlightedObj = powerPuzzleManager.HighlightableObj;
            }
            else if (targetTransform.TryGetComponent<KeycodeReceiver>(out var keycodeReceiver))
            {
                if (!keycodeReceiver.CodeHasBeenAccepted && keycodeReceiver.ShouldHighlight)
                {
                    currentHighlightedObj = targetTransform;
                }
            }
            else if (targetTransform.TryGetComponent<ShootableObject>(out var shootableObj))
            {
                //_gunController.closeObj = shootableObj;
                //currentHighlightedObj = targetTransform;
            }
            else if (!ShouldSkipHighlighting(targetTransform))
            {
                Debug.Log("Fallback highlight on: " + targetTransform.name);
                currentHighlightedObj = targetTransform;
            }
        }

        targetTransform = other.transform;
        if (!targetTransform.TryGetComponent<IItemReceiver>(out var itemReceiver) && targetTransform.parent)
        {
            targetTransform = targetTransform.parent;
            targetTransform.TryGetComponent(out itemReceiver);
        }

        if (itemReceiver != null)
        {
            _currentItemReceiver = itemReceiver;
            if (!_currentItemReceiver.ItemHasBeenReceived)
            {
                //currentHighlightedObj = targetTransform;
            }
        }
    }

    private bool ShouldSkipHighlighting(Transform target)
    {
        return target.GetComponentInParent<PowerPuzzleTile>() != null ||
               target.GetComponentInChildren<PowerPuzzleTile>() != null ||
               target.GetComponent<PowerPuzzleTile>() != null ||
               target.GetComponentInParent<ExitDoor>() != null ||
               target.GetComponentInChildren<ExitDoor>() != null ||
               target.GetComponent<ExitDoor>() != null;
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
                currentHighlightedObj = null;
                _gunController.closeObj = null;
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
                currentHighlightedObj = null;
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

    private void ClearCurrentItemReceiver(IItemReceiver receiver)
    {
        if (_currentItemReceiver == receiver)
        {
            _currentItemReceiver = null;
        }
    }
    
}


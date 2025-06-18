using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public bool isTestingHub;
    public bool isTestingTank;
    public bool isTestingPlatform;
    public bool isTestingFPS;
    
    private float _movementInput;
    private float _rotationInput;
    private Vector2 _lookInput;

    public float MovementInput => _movementInput;
    public float RotationInput => _rotationInput;
    public Vector2 LookInput => _lookInput;
    
    [HideInInspector]
    public bool _isCrouching { get; private set; }

    private Rigidbody _rb;

    public IPlayerMode CurrentMode { get; set; }
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context) => _movementInput = context.ReadValue<float>();
    public void OnRotate(InputAction.CallbackContext context) => _rotationInput = context.ReadValue<float>();
    public void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

    public void OnAim(InputAction.CallbackContext context)
    {
        CurrentMode?.Aim(context);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CurrentMode?.Attack();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CurrentMode?.Jump();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isCrouching = !_isCrouching;
            CurrentMode?.Crouch(_isCrouching);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        CurrentMode?.Sprint(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentInteractable?.Interact(transform);
        }
    }

    public void OnSpecial(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CurrentMode?.Special();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.TogglePauseGame();
        }
    }

    private void FixedUpdate()
    {
        if (InputManager.Instance.IsInUI) return;
        CurrentMode?.Tick();
        CurrentMode?.Move(_rb, _movementInput, transform);
        CurrentMode?.Rotate(_rotationInput, transform);
        CurrentMode?.Look(_lookInput, transform);
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


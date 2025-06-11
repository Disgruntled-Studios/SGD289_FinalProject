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
    
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    [HideInInspector]
    public bool _isCrouching { get; private set; }

    private Rigidbody _rb;

    public IPlayerMode CurrentMode { get; set; }
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context) => _movementInput = context.ReadValue<Vector2>();
    public void OnRotate(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

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
            _isCrouching = _isCrouching;
            CurrentMode?.Crouch(_isCrouching);
        }
        else if (context.canceled)
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

    private void FixedUpdate()
    {
        CurrentMode?.Move(_rb, _movementInput, transform);
        CurrentMode?.Rotate(_lookInput, transform);
    }

    private void Update()
    {
        CurrentMode?.Tick();
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


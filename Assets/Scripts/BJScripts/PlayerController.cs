using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private bool _crouchPressed;

    private Rigidbody _rb;
    
    public IPlayerMode CurrentMode { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context) => _movementInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentMode?.Jump();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        _crouchPressed = context.ReadValue<float>() > 0.5f;
        CurrentMode?.Crouch(_crouchPressed);
    }

    private void FixedUpdate()
    {
        CurrentMode?.Move(_rb, _movementInput, transform);
    }

    private void Update()
    {
        CurrentMode?.Look(_lookInput);
        CurrentMode?.Tick();
    }
}


using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private bool _crouchPressed;
    private GunFunctions _gunReference;

    private Rigidbody _rb;

    public IPlayerMode CurrentMode { get; set; }

    private void Awake()
    {
        if (GetComponent<GunFunctions>())
        {
            _gunReference = GetComponent<GunFunctions>();
        }
        _rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context) => _movementInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

    public void OnAim(InputAction.CallbackContext context)
    {
        TankPlayerMode tank = CurrentMode.ConvertTo<TankPlayerMode>();
        if (context.performed)
        {
            _gunReference.StartGunAim();
            tank.ToggleRotationSpeed();
        }
        else if (context.canceled)
        {
            _gunReference.EndGunAim();
            tank.ToggleRotationSpeed();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _gunReference.Shoot();
        }
    }

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


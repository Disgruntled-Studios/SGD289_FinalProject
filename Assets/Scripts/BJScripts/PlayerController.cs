using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool isTestingHub;
    public bool isTestingTank;
    public bool isTestingPlatform;
    public bool isTestingStealth;

    public UnitHealth playerHealth;
    [SerializeField] private float maxHealth; 
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private bool _isCrouching;
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
        playerHealth = new UnitHealth(maxHealth);
    }

    public void OnMove(InputAction.CallbackContext context) => _movementInput = context.ReadValue<Vector2>();
    public void OnRotate(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

    public void OnAim(InputAction.CallbackContext context)
    {
        TankPlayerMode tankRef = new TankPlayerMode();
        bool isInTank = false;
        
        switch (CurrentMode)
        {
            case TankPlayerMode:
                tankRef = CurrentMode.ConvertTo<TankPlayerMode>();
                isInTank = true;
                break;
        }

        if (context.performed && isInTank)
        {
            _gunReference.StartGunAim();
            tankRef.ToggleRotationSpeed();
        }
        else if (context.canceled && isInTank)
        {
            _gunReference.EndGunAim();
            tankRef.ToggleRotationSpeed();
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

    private void FixedUpdate()
    {
        CurrentMode?.Move(_rb, _movementInput, transform);
    }

    private void Update()
    {
        CurrentMode?.Rotate(_lookInput);
        CurrentMode?.Tick();
    }
}


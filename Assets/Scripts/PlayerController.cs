using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float _movementSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Camera _mainCam;

    private Rigidbody _rb;
    private Vector2 _inputVector;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }
    
    private void HandleMovement()
    {
        var move = new Vector3(_inputVector.x, 0f, _inputVector.y) * _movementSpeed;
        _rb.linearVelocity = new Vector3(move.x, _rb.linearVelocity.y, move.z);
    }

    private void HandleRotation()
    {
        var playerScreenPos = _mainCam.WorldToScreenPoint(transform.position);
        var mouseScreenPos = Mouse.current.position.ReadValue();

        var direction = (mouseScreenPos - new Vector2(playerScreenPos.x, playerScreenPos.y)).normalized;

        var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        var targetRotation = Quaternion.Euler(0f, angle, 0f);
        _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
    }
}

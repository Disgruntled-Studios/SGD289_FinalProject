using UnityEngine;

public class StealthPlayerMode : IPlayerMode
{
    private readonly float _speed;
    private readonly Transform _playerTransform;
    private bool _isCrouching;

    public StealthPlayerMode(float speed, Transform playerTransform)
    {
        _speed = speed;
        _playerTransform = playerTransform;
    }
    
    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        var currentSpeed = _isCrouching ? _speed * 0.5f : _speed;

        var forward = _playerTransform.forward;
        var right = _playerTransform.right;

        var moveDirection = forward * input.y + right * input.x;
        var velocity = new Vector3(moveDirection.x * currentSpeed, rb.linearVelocity.y, moveDirection.z * currentSpeed);
        rb.linearVelocity = velocity;
    }

    public void Look(Vector2 input)
    {
        const float sensitivity = 2f;
        _playerTransform.Rotate(Vector3.up, input.x * sensitivity);
    }

    public void Jump() { } // Not used in stealth

    public void Crouch(bool isPressed)
    {
        _isCrouching = isPressed;
    }

    public void Tick() { } // Not used in stealth
}

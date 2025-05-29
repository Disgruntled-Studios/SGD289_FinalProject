using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformPlayerMode : IPlayerMode
{
    private readonly Rigidbody _rb;
    private readonly float _speed;
    private readonly float _jumpForce;
    private readonly Transform _playerTransform;
    
    public PlatformPlayerMode(Rigidbody playerRb, float speed, float jumpForce, Transform playerTransform)
    {
        _rb = playerRb;
        _speed = speed;
        _jumpForce = jumpForce;
        _playerTransform = playerTransform;
    }
    
    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        var moveDirection = _playerTransform.right * input.x;
        var velocity = new Vector3(moveDirection.x * _speed, _rb.linearVelocity.y, 0);
        _rb.linearVelocity = velocity;

        if (Mathf.Abs(input.x) > 0.01f)
        {
            var newScale = _playerTransform.localScale;
            newScale.x = Mathf.Sign(input.x) * Mathf.Abs(newScale.x);
            _playerTransform.localScale = newScale;
        }
    }

    public void Rotate(Vector2 input) { } // Not used in platformer

    public void Jump()
    {
        if (Mathf.Abs(_rb.linearVelocity.y) < 0.05f)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    public void Crouch(bool isPressed) { } // Not used in platformer

    public void Tick() { } // Not used in platformer
    public void Aim(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }
}

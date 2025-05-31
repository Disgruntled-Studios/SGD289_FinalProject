using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class HubMovementMode : IPlayerMode
{
    private readonly float _speed;
    private readonly float _rotationSpeed;

    public HubMovementMode(float speed, float rotationSpeed = 10f)
    {
        _speed = speed;
        _rotationSpeed = rotationSpeed;
    }

    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        // No movement? Exit early
        if (input == Vector2.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        // Calculate move direction in world space
        var moveDir = context.TransformDirection(new Vector3(input.x, 0f, input.y)).normalized;

        // Apply movement
        var velocity = moveDir * _speed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    public void Rotate(Vector2 input, Transform context)
    {
        // Use right stick (or mouse) for rotation
        if (input.sqrMagnitude < 0.001f) return;

        // Smoothly rotate towards the look direction
        var targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
        targetAngle += Camera.main.transform.eulerAngles.y; // Add camera rotation for third-person feel

        var targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        context.rotation = Quaternion.Slerp(context.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }

    public void Jump()
    {
        return;
    } // No jumping in hub
    

    public void Crouch(bool isPressed)
    {
        return;
    } // No crouching in hub

    public void Tick() { } // No per-frame behavior right now
    public void Aim(InputAction.CallbackContext context)
    {
        return;
    }

    public void Attack()
    {
        return;
    }
}

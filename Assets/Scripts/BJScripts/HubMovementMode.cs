using TMPro;
using UnityEngine;

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
        Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;

        // Apply movement
        Vector3 velocity = moveDir * _speed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        // Rotate to face direction of movement
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        context.rotation = Quaternion.Slerp(context.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }

    public void Look(Vector2 input) { } // Look is handled by camera in hub
    public void Jump() { } // No jumping in hub
    public void Crouch(bool isPressed) { } // No crouching in hub

    public void Tick() { } // No per-frame behavior right now
}

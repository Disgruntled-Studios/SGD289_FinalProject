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
        var moveDirection = context.forward * input.y + context.right * input.x;
        moveDirection.y = 0;

        var velocity = new Vector3(moveDirection.x * _speed, rb.linearVelocity.y, moveDirection.z * _speed);
        rb.linearVelocity = velocity;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            var targetRotation = Quaternion.LookRotation(moveDirection);
            context.rotation = Quaternion.Slerp(context.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
        }
    }

    public void Look(Vector2 input) { } // Look is handled by camera in hub
    public void Jump() { } // No jumping in hub
    public void Crouch(bool isPressed) { } // No crouching in hub

    public void Tick() { } // No per-frame behavior right now
}

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
        if (Mouse.current != null)
        {
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            var groundPlane = new Plane(Vector3.up, context.position);

            if (groundPlane.Raycast(ray, out var distance))
            {
                var hitPoint = ray.GetPoint(distance);
                var direction = (hitPoint - context.position).normalized;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    var targetRotation = Quaternion.LookRotation(direction);
                    context.rotation = Quaternion.Slerp(context.rotation, targetRotation,
                        Time.fixedDeltaTime * _rotationSpeed);
                }
            }
        }
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

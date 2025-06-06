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
        if (input == Vector2.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        var camera = Camera.main;
        var camForward = camera.transform.forward;
        var camRight = camera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        var moveDirection = (camForward * input.y + camRight * input.x).normalized;

        var velocity = moveDirection * _speed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    public void Rotate(Vector2 input, Transform context)
    {
        // NEED TO DIFFERENTIATE BETWEEN MOUSE AND CONTROLLER
        if (input.sqrMagnitude < 0.001f) return;

        var yaw = input.x * _rotationSpeed;
        context.Rotate(0f, yaw, 0f);
    }

    public void Jump()
    {
        return;
    } 
    

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

    public void Special()
    {
        return;
    }
}

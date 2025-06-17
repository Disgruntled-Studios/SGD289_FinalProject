using UnityEngine;
using UnityEngine.InputSystem;

public class MirrorPlayerMode : IPlayerMode
{
    private readonly float _rotationSpeed;

    public MirrorPlayerMode(float rotationSpeed = 100f)
    {
        _rotationSpeed = rotationSpeed;
    }
    
    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        var horizontalInput = input.x;

        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            context.Rotate(Vector3.up, horizontalInput * _rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void Move(Rigidbody rb, float input, Transform context)
    {
        throw new System.NotImplementedException();
    }

    public void Rotate(float input, Transform context)
    {
        throw new System.NotImplementedException();
    }

    public void Look(Vector2 input, Transform context)
    {
        throw new System.NotImplementedException();
    }

    public void Rotate(Vector2 input, Transform context)
    {
        return;
    }

    public void Jump()
    {
        return;
    }

    public void Crouch(bool isPressed)
    {
        return;
    }

    public void Tick()
    {
        return;
    }

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
    
    public void OnModeEnter()
    {
        return;
    }
    
    public void OnModeExit()
    {
        return;
    }
    
    public void Sprint(InputAction.CallbackContext context)
    {
        return;
    }
}

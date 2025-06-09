using UnityEngine;
using UnityEngine.InputSystem;

public class TankPlayerMode : IPlayerMode
{
    private readonly float _speed;
    private readonly Transform _player;
    private readonly Rigidbody _rb;
    public float _rotationSpeed;
    private float currentRotationSpeed;
    //private bool _isAiming;
    private bool isGrounded;
    private float groundDrag = 5f;
    private float playerHeight = 2f;
    LayerMask _groundLayerMask;

    private TankGunController _tankGunReference;
    

    /// <summary>
    /// The TankPlayerMode will be the movement system based on the orientation of the player model not camera.
    /// </summary>
    /// <param name="speed">How fast the player can go.</param>
    /// <param name="player">The transform of the player object.</param>
    /// <param name="rotationSpeed">How fast the player will rotate the character model.</param>
    /// <param name="rbComponent">The Rigidbody component that is attached to the player object.</param>
    /// <param name="groundLayerMask">The Rigidbody component that is attached to the player object.</param>
    public TankPlayerMode(float speed, Transform player, float rotationSpeed, Rigidbody rbComponent, LayerMask groundLayerMask, TankGunController tankGunRef)
    {
        _speed = speed;
        _player = player;
        _rotationSpeed = rotationSpeed;
        currentRotationSpeed = rotationSpeed;
        _rb = rbComponent;
        _groundLayerMask = groundLayerMask;
        _tankGunReference = tankGunRef;
    }


    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {

        rb.AddForce(context.forward * (input.y * _speed * 10f), ForceMode.Force);
        if (input.x == 0)
        {
            _rb.angularVelocity = Vector3.zero;
        }
        else
        {
            context.transform.Rotate(new Vector3(0, input.x, 0) * currentRotationSpeed);
        }

    }

    public void ToggleRotationSpeed()
    {
        if (Mathf.Approximately(currentRotationSpeed, _rotationSpeed))
        {
            Debug.Log("Changing speed to a quarter of current speed");
            currentRotationSpeed *= 0.25f;
        }
        else
        {
            Debug.Log("Changing speed to original speed");
            currentRotationSpeed = _rotationSpeed;
        }
    }

    public void Rotate(Vector2 input, Transform context) { }

    public void Jump()
    {
        return;
    } // Not used in Tank

    public void Crouch(bool isPressed)
    {
        return;
    } // Not used in Tank
    public void Tick()
    {
        //Each frame we check if the player is grounded or not.
        isGrounded = Physics.Raycast(_player.position, Vector3.down, playerHeight * 0.5f + 0.2f, _groundLayerMask);
        Debug.DrawLine(_player.position, new Vector3(_player.position.x, (-1 * playerHeight * .5f + 0.2f), _player.position.z), Color.green);

        //If the player is grounded and has Velocity stored on the Y axis we reset their vertical velocity.X
        if (isGrounded)
        {
            _rb.linearDamping = groundDrag;
        }
        else
        {
            _rb.linearDamping = 0;
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _tankGunReference.StartGunAim();
            ToggleRotationSpeed();
        }
        
        if (context.canceled)
        {
            _tankGunReference.EndGunAim();
            ToggleRotationSpeed();
        }
    }

    public void Attack()
    {
        _tankGunReference.HandleShoot();
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

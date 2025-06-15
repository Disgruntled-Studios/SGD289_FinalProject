using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformPlayerMode : IPlayerMode
{
    private readonly Rigidbody _rb;
    private readonly float _speed;
    private readonly float _jumpForce;
    private readonly Transform _playerTransform;
    private readonly GunScript _gunScript;
    private readonly GameObject _gunModel;
    private readonly GameObject _groundCheck;
    private readonly PlayerCollisions _playerCollisions;
    private readonly GameObject _invCube;
    private bool useGravity = true;

    public PlatformPlayerMode(Rigidbody playerRb, float speed, float jumpForce, Transform playerTransform, GunScript gunScript, PlayerCollisions playerCollisions, GameObject gunModel, GameObject groundCheck, GameObject invCube)
    {
        _rb = playerRb;
        _speed = speed;
        _jumpForce = jumpForce;
        _playerTransform = playerTransform;
        _gunScript = gunScript;
        _playerCollisions = playerCollisions;
        _gunModel = gunModel;
        _groundCheck = groundCheck;
        _invCube = invCube;
    }
    
    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        if(!_playerCollisions.hasShip)
        {
            if (useGravity == false)
            {
                _rb.useGravity = true;
            }
            //want to change if player is in ship

            //if (!attacking) //gives short pause when player shoots bullet. maybe replace with anim later. //put movement in a different script instead maybe?
            //{
            var moveDirection = _playerTransform.forward * input.x;
            var velocity = new Vector3(moveDirection.x * _speed, _rb.linearVelocity.y, 0);
            _rb.linearVelocity = velocity;
            rb.AddForce(velocity * _speed * Time.deltaTime);

            if (Mathf.Abs(input.x) > 0.01f)
            {
                var newScale = _playerTransform.localScale;
                newScale.z = Mathf.Sign(input.x) * Mathf.Abs(newScale.z);
                _playerTransform.localScale = newScale;
            }
        }
        if(_playerCollisions.hasShip)
        {
            if (useGravity)
            {
                _rb.useGravity = false;
                useGravity = false;
            }

            var moveDirection = _playerTransform.forward * input.x;
            var velocity = new Vector3(moveDirection.x * _speed, _rb.linearVelocity.y, 0);
            _rb.linearVelocity = velocity;
            //rb.AddForce(velocity * _speed * Time.deltaTime);

            if (Mathf.Abs(input.x) > 0.01f)
            {
                var newScale = _playerTransform.localScale;
                newScale.z = Mathf.Sign(input.x) * Mathf.Abs(newScale.z);
                _playerTransform.localScale = newScale;
            }

        }

    }

    public void Rotate(Vector2 input, Transform context) { } // Not used in platformer

    public void Jump()
    {
        //want to change if player is in ship to also free player of ship.
        if (Mathf.Abs(_rb.linearVelocity.y) < 0.01f)
        {
            if (_playerCollisions.hasShip)
            {
                _playerCollisions.ExitShip();
            }
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    public void Crouch(bool isPressed) { } // Not used in platformer

    public void Tick() { } // Not used in platformer
    public void Aim(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    //left button
    public void Attack()
    {
        //throw new System.NotImplementedException();
        _gunScript.Shoot();
    }

    public void Special()
    {
        return;
    }

    public void OnModeEnter()
    {
        if (_playerCollisions)
        {
            _playerCollisions.enabled = true;
        }
        _gunModel.SetActive(true);
        // Laser is disabled initially by default
        _gunScript.ToggleLineRenderer(false);
        _groundCheck.SetActive(true);
        useGravity = true;

    }

    public void OnModeExit()
    {
        _groundCheck.SetActive(false);
        _invCube.SetActive(false);

        // Reset scale if player is flipped
        if (_playerTransform.localScale.z < 1)
        {
            var scale = _playerTransform.localScale;
            scale.z = 1;
            _playerTransform.localScale = scale;
        }
        _playerCollisions.enabled = false;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        return;
    }






}

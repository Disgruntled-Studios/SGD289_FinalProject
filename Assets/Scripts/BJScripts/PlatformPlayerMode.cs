using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformPlayerMode : IPlayerMode
{
    private readonly Rigidbody _rb;
    private readonly float _speed;
    private readonly float _jumpForce;
    private readonly Transform _playerTransform;

    //Plans for script writing:
    //plan to add jumping
    //add health and scoring as well, though scoring in another script perhaps.
    //player can destroy small enemies by jumping on them (which causes player to bounce and bounce higher if they click jump button at same time)
    //if time, will also adding kicking which allows player to kick enemies or objects
    //if player destroys a lightning ship, the enemy will disappear or fall off and player can jump onto their ship
    //and ride it for 20 (adjustable in outliner) seconds before it flashes and disappears.
    //While riding ship, shooting button instead fires a lazer beam forwards same as ship enemy in firing mode 
    //Enemy ships in the game are either in dive bomb mode or firing mode. Dive bomb ships dive straight at player and can travel through platforms and other terrain.
    //They are also invincible in this mode, but in firing mode, they travel along where player is and also can travel through platforms and other terrain. 
    //Player cannot dive through terrain while riding them though and perhaps cannot shoot them while they are in terrain (error check later)
    //Player can access some otherwise inaccessible locations in the sky while riding the ship.
    //Health system. Heath is represented by 3 astronaut helmets in top left corner of screen. There is a score in top right.
    //Player collects alien coins throughout the level that loook like mini spaceships.

    //sets parameters
    public PlatformPlayerMode(Rigidbody playerRb, float speed, float jumpForce, Transform playerTransform)
    {
        _rb = playerRb;
        _speed = speed;
        _jumpForce = jumpForce;
        _playerTransform = playerTransform;
    }
    
    //moves player
    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        Debug.Log("moved");
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

    public void Rotate(Vector2 input) 
    {
        Debug.Log("rotate");

    } // Not used in platformer so left blank

    //Jump, uses rigidbody
    public void Jump()
    {
        if (Mathf.Abs(_rb.linearVelocity.y) < 0.05f)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    
    //might use in platformer
    public void Crouch(bool isPressed) 
    {
        Debug.Log("Crouch");
    } // Not used in platformer


    public void Tick() 
    {
        Debug.Log("Tick function");
    } // Not used in platformer
    public void Aim(InputAction.CallbackContext context)
    {
        Debug.Log("Aiming");
        //throw new System.NotImplementedException();
    }

    public void Interact(InputAction.CallbackContext context) //would like to use the f key to kick
    {
        Debug.Log("Interact");
       // throw new System.NotImplementedException();
    }

    public void Attack() //shoot
    {
        Debug.Log("Attack");
        //throw new System.NotImplementedException();
    }
    
}

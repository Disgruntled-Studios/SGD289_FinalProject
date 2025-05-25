using UnityEngine;

public class SurvivalPlayerMode : IPlayerMode
{
    private readonly float _speed;
    private readonly Transform _player;
    private float _pitch;

    public SurvivalPlayerMode(float speed, Transform player)
    {
        _speed = speed;
        _player = player;
    }

    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        var forward = _player.forward;
        var right = _player.right;

        var moveDirection = forward * input.y + right * input.x;
        var velocity = new Vector3(moveDirection.x * _speed, rb.linearVelocity.y, moveDirection.z * _speed);
        rb.linearVelocity = velocity;
    }

    public void Look(Vector2 input)
    {
        const float sensitivity = 2f;
        _player.Rotate(Vector3.up, input.x * sensitivity);
    }

    public void Jump() { } // Not used in FPS
    public void Crouch(bool isPressed) { } // Not used in FPS
    public void Tick() { } // Not used in FPS
}

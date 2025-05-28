using UnityEngine;

public interface IPlayerMode
{
    void Move(Rigidbody rb, Vector2 input, Transform context); // How the player will move
    void Rotate(Vector2 input); // How the model will rotate, cameras handle viewports
    void Jump(); // How the player jumps
    void Crouch(bool isPressed); // How the player crouches
    void Tick(); // Per-frame behavior if necessary
}

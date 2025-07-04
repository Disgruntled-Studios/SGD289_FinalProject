using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerMode
{
    void Move(Rigidbody rb, float input, Transform context); // How the player will move
    void Rotate(float input, Transform context); // How the model will rotate
    void Look(Vector2 input, Transform context);
    void Jump(); // How the player jumps
    void Crouch(bool isPressed); // How the player crouches
    void Sprint(InputAction.CallbackContext context); // How the player should sprint (if needed)
    void Tick(); // Per-frame behavior if necessary
    void Aim(InputAction.CallbackContext context); // How the player should aim
    void Attack(); // How the player should attack
    void Special(); // Special mechanic for each level!
    void OnModeEnter(); // Default behavior for entering this mode
    void OnModeExit(); // Default behavior for exiting this mode
}

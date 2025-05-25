using UnityEngine;

public interface IPlayerMode
{
    void Move(Rigidbody rb, Vector2 input, Transform context);
    void Look(Vector2 input);
    void Jump();
    void Crouch(bool isPressed);
    void Tick(); // Per-frame behavior if necessary
}

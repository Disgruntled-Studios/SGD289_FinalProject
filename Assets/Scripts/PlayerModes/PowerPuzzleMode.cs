using UnityEngine;
using UnityEngine.InputSystem;

public class PowerPuzzleMode : IPlayerMode
{
    private TileSelection playerSelection;

    public PowerPuzzleMode(TileSelection _playerSelection)
    {
        playerSelection = _playerSelection;
    }
    public void Aim(InputAction.CallbackContext context)
    {
        return;
    }

    public void Attack()
    {
        return;
    }

    public void Crouch(bool isPressed)
    {
        return;
    }

    public void Jump()
    {
        return;
    }

    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        playerSelection.MoveSelection(input.x, input.y);
    }

    public void Rotate(Vector2 input, Transform context)
    {
        return;
    }

    public void Tick()
    {
        return;
    }

    public void Special()
    {
        return;
    }
}

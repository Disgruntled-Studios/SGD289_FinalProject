using UnityEngine;
using UnityEngine.InputSystem;

public class TESTPuzzleControlScheme : MonoBehaviour
{
    public void RotateTileRight()
    {
        if (InputManager.Instance.IsInPuzzle)
        {
            Debug.Log("Rotate right");
        }
    }

    public void RotateTileLeft()
    {
        if (InputManager.Instance.IsInPuzzle)
        {
            Debug.Log("Rotate left");
        }
    }

    public void MoveSouth()
    {
        if (InputManager.Instance.IsInPuzzle)
        {
            Debug.Log("Move South");
        }
    }

    public void MoveNorth()
    {
        if (InputManager.Instance.IsInPuzzle)
        {
            Debug.Log("Move North");
        }
    }

    public void MoveWest()
    {
        if (InputManager.Instance.IsInPuzzle)
        {
            Debug.Log("Move west");
        }
    }

    public void MoveEast()
    {
        if (InputManager.Instance.IsInPuzzle)
        {
            Debug.Log("Move east");
        }
    }

    public void ExitPuzzle()
    {
        if (InputManager.Instance.IsInPuzzle)
        {
            Debug.Log("Exit puzzle");
        }
    }
}

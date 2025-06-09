using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PowerPuzzleInteration : MonoBehaviour, IInteractable
{
    private InputAction move_N;
    private InputAction move_S;
    private InputAction move_W;
    private InputAction move_E;
    PowerPuzzleManager manager;
    public GameCamera sceneCam;

    void Awake()
    {
        manager = GetComponentInChildren<PowerPuzzleManager>();
    }

    void OnEnable()
    {
        move_N = InputManager._inputActions.PuzzleMap.MoveNorth;
        move_S = InputManager._inputActions.PuzzleMap.MoveSouth;
        move_W = InputManager._inputActions.PuzzleMap.MoveWest;
        move_E = InputManager._inputActions.PuzzleMap.MoveEast;
    }

    void OnDisable()
    {
        //InputManager._inputActions.PuzzleMap.ExitPuzzle.performed -= manager.ExitPuzzle;
    }

    public void Interact(Transform player)
    {
        Debug.Log("Interact function called");
        if (!manager.isPuzzledone)
        {
            Debug.Log("Starting puzzle");
            manager.outOfPuzzleWorld = (World)GameManager.Instance.CurrentWorld;
            manager.outOfPuzzleCamID = sceneCam.CameraID;
            //GameManager.Instance.currentTileSelection = tileSelection;
            GameManager.Instance.SwitchPlayerMode(World.Puzzle);
            CameraManager.Instance.TrySwitchToCamera("PowerPuzzleCam");
        }
        else
        {
            Debug.Log("Cannot Enter puzzle because it has already been solved");
        }
    }

    public void OnExit()
    {
        return;
    }
}

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
        move_N = InputManager.Instance.PlayerInput.PuzzleMap.MoveNorth;
        move_S = InputManager.Instance.PlayerInput.PuzzleMap.MoveSouth;
        move_W = InputManager.Instance.PlayerInput.PuzzleMap.MoveWest;
        move_E = InputManager.Instance.PlayerInput.PuzzleMap.MoveEast;
    }

    void OnDisable()
    {
        //InputManager.Instance.PlayerInput.PuzzleMap.ExitPuzzle.performed -= manager.ExitPuzzle;
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

using Unity.Cinemachine;
using UnityEngine;

public class PowerPuzzleInteration : MonoBehaviour, IInteractable
{
    PowerPuzzleManager manager;
    TileSelection tileSelection;
    public GameCamera sceneCam;

    void Awake()
    {
        manager = GetComponentInChildren<PowerPuzzleManager>();
        tileSelection = GetComponentInChildren<TileSelection>();
    }

    public void Interact(Transform player)
    {
        Debug.Log("Interact function called");
        if (!manager.isPuzzledone)
        {
            Debug.Log("Starting puzzle");
            manager.outOfPuzzleWorld = (World)GameManager.Instance.CurrentWorld;
            manager.outOfPuzzleCamID = sceneCam.CameraID;
            GameManager.Instance.currentTileSelection = tileSelection;
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

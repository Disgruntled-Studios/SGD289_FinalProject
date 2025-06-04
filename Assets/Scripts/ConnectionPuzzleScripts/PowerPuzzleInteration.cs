using Unity.Cinemachine;
using UnityEngine;

public class PowerPuzzleInteration : MonoBehaviour, IInteractable
{
    PowerPuzzleManager manager;
    TileSelection tileSelection;
    public CinemachineCamera sceneCam;

    void Awake()
    {
        manager = GetComponentInChildren<PowerPuzzleManager>();
        tileSelection = GetComponentInChildren<TileSelection>();
    }

    public void Interact(Transform player)
    {
        if (!manager.isPuzzledone)
        {
            manager.outOfPuzzleWorld = (World)GameManager.Instance.CurrentWorld;
            manager.outOfPuzzleCamID = sceneCam.gameObject.name;
            GameManager.Instance.currentTileSelection = tileSelection;
            GameManager.Instance.SwitchPlayerMode(World.Puzzle);
            CameraManager.Instance.TrySwitchToCamera("PowerPuzzleCam");
        }
    }

    public void OnExit()
    {
        return;
    }
}

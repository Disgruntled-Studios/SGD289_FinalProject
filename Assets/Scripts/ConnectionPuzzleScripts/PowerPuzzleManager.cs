using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PowerPuzzleManager : MonoBehaviour, IInteractable
{

    public PowerPuzzleTile powerNode;
    public PowerPuzzleTile recieverNode;
    public List<PowerPuzzleTile> tiles;
    public UnityEvent onPuzzleCompletion;
    public TileSelection tileSelection;
    public World outOfPuzzleWorld;
    public string outOfPuzzleCamID;
    public GameCamera sceneCam;
    public bool isPuzzledone;

    void Awake()
    {
        for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
        {
            if (transform.GetChild(i).GetComponent<PowerPuzzleTile>().isPowerNode)
            {
                powerNode = transform.GetChild(i).GetComponent<PowerPuzzleTile>();
            }
            else if (transform.GetChild(i).GetComponent<PowerPuzzleTile>().isRecieverNode)
            {
                recieverNode = transform.GetChild(i).GetComponent<PowerPuzzleTile>();
            }
            tiles.Add(transform.GetChild(i).GetComponent<PowerPuzzleTile>());
        }
        isPuzzledone = false;
        tileSelection = GetComponentInChildren<TileSelection>();
    }

    void Update()
    {
        if (recieverNode.isPowered && recieverNode.isConnected && isPuzzledone == false && outOfPuzzleWorld != World.Puzzle)
        {
            Debug.Log("Puzzle complete");
            onPuzzleCompletion.Invoke();
            CameraManager.Instance.TrySwitchToCamera(outOfPuzzleCamID);
            ExitPuzzle();
        }

        if (!powerNode.isConnected) CheckTilesConnection();
    }

    public void ExitPuzzle()
    {
        //GameManager.Instance.SwitchPlayerMode(outOfPuzzleWorld);
        CameraManager.Instance.TrySwitchToCamera(outOfPuzzleCamID);
        InputManager.ToggleActionMap(InputManager._inputActions.Player);
    }

    public void CheckTilesConnection()
    {
        foreach (PowerPuzzleTile tile in tiles)
        {
            if (tile != powerNode && tile.isPowered)
            {
                Debug.Log(tile + " is not a power node turning it off");
                tile.isPowered = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PuzzleController>())
        {
            other.gameObject.GetComponent<PuzzleController>().SetVars(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PuzzleController>())
        {
            other.gameObject.GetComponent<PuzzleController>().WipeVars();
        }
    }

    public void MoveSelection(int dir)
    {
        switch (dir)
        {
            case 1:
                //If its North
                if (tileSelection.transform.localPosition.x <= tileSelection.xLimit)
                {
                    tileSelection.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 2, 0);
                }
                break;
            case 2:
                //South
                if (tileSelection.transform.localPosition.x >= -tileSelection.xLimit)
                {
                    tileSelection.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 2, 0);
                }
                break;
            case 3:
                //West
                if (transform.localPosition.y <= tileSelection.yLimit)
                {
                    tileSelection.transform.localPosition = new Vector3(transform.localPosition.x + 2, transform.localPosition.y, 0);
                }
                break;
            case 4:
                //East
                if (transform.localPosition.y >= -tileSelection.yLimit)
                {
                    tileSelection.transform.localPosition = new Vector3(transform.localPosition.x - 2, transform.localPosition.y, 0);
                }
                break;
        }
    }

    public void RotateTile(bool rotateRight)
    {
        if (rotateRight)
        {
            tileSelection.selectedOBJ.transform.Rotate(0, 0, 90f);
        }
        else
        {
            tileSelection.selectedOBJ.transform.Rotate(0, 0, -90f);
        }
        CheckTilesConnection();
    }

    public void Interact(Transform player)
    {
        Debug.Log("Interact function called");
        if (!isPuzzledone)
        {
            Debug.Log("Starting puzzle");
            //outOfPuzzleWorld = (World)GameManager.Instance.CurrentWorld;
            outOfPuzzleCamID = sceneCam.CameraID;
            //GameManager.Instance.currentTileSelection = tileSelection;
            InputManager.ToggleActionMap(InputManager._inputActions.PuzzleMap);
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

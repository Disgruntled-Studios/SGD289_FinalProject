using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PowerPuzzleManager : MonoBehaviour, IInteractable
{

    [HideInInspector]
    public PowerPuzzleTile powerNode;
    [HideInInspector]
    public PowerPuzzleTile recieverNode;
    public List<PowerPuzzleTile> tiles;
    public UnityEvent onPuzzleCompletion;
    public TileSelection tileSelection;
    public GameCamera sceneCam;
    public GameCamera puzzleCam;
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
        //tileSelection = GetComponentInChildren<TileSelection>();
    }

    void Update()
    {
        if (recieverNode.isPowered && recieverNode.isConnected && isPuzzledone == false)
        {
            isPuzzledone = true;
            //Debug.Log("Puzzle complete");
            onPuzzleCompletion.Invoke();
            CameraManager.Instance.TrySwitchToCamera(sceneCam.CameraID);
            ExitPuzzle();
        }

        if (!powerNode.isConnected) CheckTilesConnection();
    }

    public void ExitPuzzle()
    {
        //GameManager.Instance.SwitchPlayerMode(outOfPuzzleWorld);
        CameraManager.Instance.TrySwitchToCamera(sceneCam.CameraID);
        InputManager.Instance.SwitchToDefaultInput();
        PuzzleUI_Manager.Instance.SetPuzzlePanel(false);
    }

    public void CheckTilesConnection()
    {
        foreach (PowerPuzzleTile tile in tiles)
        {
            if (tile != powerNode && tile.isPowered)
            {
                //Debug.Log(tile + " is not a power node turning it off");
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
        if (!InputManager.Instance.IsInPuzzle) return;

        switch (dir)
        {
            case 1:
                //If its North
                if (tileSelection.transform.localPosition.y < tileSelection.yLimit)
                {
                    tileSelection.transform.localPosition = new Vector3(tileSelection.transform.localPosition.x, tileSelection.transform.localPosition.y + 2, 0);
                }
                break;
            case 2:
                //South
                if (tileSelection.transform.localPosition.y > -tileSelection.yLimit)
                {
                    tileSelection.transform.localPosition = new Vector3(tileSelection.transform.localPosition.x, tileSelection.transform.localPosition.y - 2, 0);
                }
                break;
            case 3:
                //West
                if (tileSelection.transform.localPosition.x > -tileSelection.xLimit)
                {
                    tileSelection.transform.localPosition = new Vector3(tileSelection.transform.localPosition.x - 2, tileSelection.transform.localPosition.y, 0);
                }
                break;
            case 4:
                //East
                if (tileSelection.transform.localPosition.x < tileSelection.xLimit)
                {
                    tileSelection.transform.localPosition = new Vector3(tileSelection.transform.localPosition.x + 2, tileSelection.transform.localPosition.y, 0);
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

    public void Interact(Transform player, PlayerInventory inventory)
    {
        //Debug.Log("Interact function called");
        if (!isPuzzledone)
        {
            InputManager.Instance.SwitchToPuzzleInput();
            CameraManager.Instance.TrySwitchToCamera(puzzleCam.CameraID);
            PuzzleUI_Manager.Instance.SetPuzzlePanel(true);

        }
        else
        {
            Debug.LogWarning("Cannot Enter puzzle because it has already been solved");
        }
    }

    public void OnEnter()
    {
        return;
    }

    public void OnExit()
    {
        return;
    }

}

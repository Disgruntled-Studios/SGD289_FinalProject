using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerPuzzleManager : MonoBehaviour
{

    public PowerPuzzleTile powerNode;
    public PowerPuzzleTile recieverNode;
    public List<PowerPuzzleTile> tiles;
    public UnityEvent onPuzzleCompletion;
    public TileSelection playerSelection;
    public World outOfPuzzleWorld;
    public string outOfPuzzleCamID;
    private Camera cam;
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
        cam = Camera.main;
        isPuzzledone = false;
    }

    void Update()
    {
        if (recieverNode.isPowered && recieverNode.isConnected && isPuzzledone == false && outOfPuzzleWorld != World.Puzzle)
        {
            Debug.Log("Puzzle complete");
            onPuzzleCompletion.Invoke();
            GameManager.Instance.SwitchPlayerMode(outOfPuzzleWorld);
            CameraManager.Instance.TrySwitchToCamera(outOfPuzzleCamID);
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(cam.gameObject.transform.position, mousePos - cam.gameObject.transform.position, Color.blue);

        if (!powerNode.isConnected) CheckTilesConnection();

    }

    void CheckTilesConnection()
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

    void RotateTile(GameObject tileRef, bool rotateRight)
    {
        if (rotateRight)
        {
            Debug.Log("Rotating right");
            tileRef.transform.Rotate(0, 0, 90f);
        }
        else
        {
            Debug.Log("Rotating left");
            tileRef.transform.Rotate(0, 0, -90f);
        }
        CheckTilesConnection();
    }

}

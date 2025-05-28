using System.Collections.Generic;
using UnityEngine;

public class PowerPuzzleManager : MonoBehaviour
{

    public PowerPuzzleTile powerNode;
    public List<PowerPuzzleTile> tiles;

    void Awake()
    {
        for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
        {
            if (transform.GetChild(i).GetComponent<PowerPuzzleTile>().isPowerNode)
            {
                powerNode = transform.GetChild(i).GetComponent<PowerPuzzleTile>();
            }
            tiles.Add(transform.GetChild(i).GetComponent<PowerPuzzleTile>());
        }
    }

    void Update()
    {
        if (!powerNode.isConnected)
        {
            Debug.Log("PowerNode Disconnected resetting all tiles");
            foreach (PowerPuzzleTile tile in tiles)
            {
                if (tile != powerNode && tile.isPowered)
                {
                    Debug.Log(tile + " is not a power node turning it off");
                    tile.isPowered = false;
                    tile.ToggleConnectionMaterial(false);
                }
            }
        }
    }

}

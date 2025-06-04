using UnityEngine;

public class ConnectorScript : MonoBehaviour
{
    public PowerPuzzleTile currentConnection;
    public PowerPuzzleTile parentTile;

    void Awake()
    {
        parentTile = transform.parent.GetComponent<PowerPuzzleTile>();
    }

}

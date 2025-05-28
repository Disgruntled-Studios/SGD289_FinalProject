using UnityEngine;

public class ConnectorScript : MonoBehaviour
{
    public PowerPuzzleTile currentConnection;
    public PowerPuzzleTile parentTile;

    void Awake()
    {
        parentTile = transform.parent.GetComponent<PowerPuzzleTile>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>())
        {
            parentTile.isConnected = true;
            PowerPuzzleTile tileRef = other.gameObject.GetComponentInParent<PowerPuzzleTile>();
            /*
            if (tileRef.isPowerNode || tileRef.isPowered)
            {
                //powerDependant = tileRef;
                Debug.Log("Power is on for " + name);
                isPowered = true;
                ToggleConnectionMaterial(true);
            }
            */

            if (parentTile.isPowerNode && !tileRef.isPowered || parentTile.isPowered && !tileRef.isPowered)
            {
                currentConnection = tileRef;
                tileRef.powerDependant = parentTile;
                tileRef.isPowered = true;
                tileRef.ToggleConnectionMaterial(true);
            }
            

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>())
        {
            parentTile.isConnected = true;
            PowerPuzzleTile tileRef = other.gameObject.GetComponentInParent<PowerPuzzleTile>();
            /*
            if (tileRef.isPowered || tileRef.isPowerNode)
            {
                isPowered = true;
                ToggleConnectionMaterial(true);
            }
            

            if (parentTile.isPowerNode && !tileRef.isPowered || parentTile.isPowered && !tileRef.isPowered)
            {
                tileRef.powerDependant = parentTile;
                tileRef.isPowered = true;
                tileRef.ToggleConnectionMaterial(true);
            }
            */
        }
    }

    void OnTriggerExit(Collider other)
    {
        parentTile.isConnected = false;
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>() && !parentTile.isPowerNode)
        {
            parentTile.isPowered = false;
            other.gameObject.GetComponentInParent<PowerPuzzleTile>().ToggleConnectionMaterial(false);
        }
    }
}

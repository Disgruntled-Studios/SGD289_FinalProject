using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerPuzzleTile : MonoBehaviour
{
    public bool isPowered;
    public bool isPowerNode;
    public bool isConnected;
    public bool isRecieverNode;
    public Material offMaterial;
    public Material onMaterial;
    [HideInInspector]
    public List<GameObject> connectors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
        {
            connectors.Add(transform.GetChild(i).transform.gameObject);
        }
    }

    void LateUpdate()
    {
        if (!isPowered)
        {
            ToggleConnectionMaterial(false);
        }
        else
        {
            ToggleConnectionMaterial(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isPowered && isConnected) return;
        
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>())
        {
            isConnected = true;
            PowerPuzzleTile tileRef = other.gameObject.GetComponentInParent<PowerPuzzleTile>();

            if (tileRef.isPowered || tileRef.isPowerNode)
            {
                isPowered = true;
            }

        }
    }

    public void ToggleConnectionMaterial(bool _isPowered)
    {
        if (_isPowered)
        {
            foreach (GameObject gameObject in connectors)
            {
                gameObject.GetComponent<MeshRenderer>().material = onMaterial;
            }
        }
        else
        {
            foreach (GameObject gameObject in connectors)
            {
                gameObject.GetComponent<MeshRenderer>().material = offMaterial;
            }
        }
    }
}

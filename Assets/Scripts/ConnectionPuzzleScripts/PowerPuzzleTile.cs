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
    public PowerPuzzleTile powerDependant;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
        {
            connectors.Add(transform.GetChild(i).transform.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (powerDependant != null && !powerDependant.isPowered)
            {
                isPowered = false;
            }
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>())
        {
            isConnected = true;
            PowerPuzzleTile tileRef = other.gameObject.GetComponentInParent<PowerPuzzleTile>();
            
            if (tileRef.isPowerNode || tileRef.isPowered)
            {
                //powerDependant = tileRef;
                Debug.Log("Power is on for " + name);
                isPowered = true;
                ToggleConnectionMaterial(true);
            }
            

            if (this.isPowerNode && !tileRef.isPowered || this.isPowered && !tileRef.isPowered)
            {
                tileRef.powerDependant = this;
                isPowered = true;
                ToggleConnectionMaterial(true);
            }
            

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>())
        {
            isConnected = true;
            PowerPuzzleTile tileRef = other.gameObject.GetComponentInParent<PowerPuzzleTile>();
            
            if (tileRef.isPowered || tileRef.isPowerNode)
            {
                isPowered = true;
                ToggleConnectionMaterial(true);
            }
            

            if (this.isPowerNode && !tileRef.isPowered || this.isPowered && !tileRef.isPowered)
            {
                tileRef.powerDependant = this;
                tileRef.isPowered = true;
                ToggleConnectionMaterial(true);
            }

            if (powerDependant != null && !powerDependant.isPowered)
            {
                isPowered = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        isConnected = false;
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>())
        {
            isPowered = false;
            ToggleConnectionMaterial(false);
        }
    }
    */

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

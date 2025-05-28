using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerPuzzleTile : MonoBehaviour
{
    public bool isPowered;
    public bool isPowerNode;
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

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>())
        {
            PowerPuzzleTile tileRef = other.gameObject.GetComponentInParent<PowerPuzzleTile>();
            if (tileRef.isPowerNode)
            {
                Debug.Log("Power is on for " + name);
                isPowered = true;
                ToggleConnectionMaterials(true);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>() && isPowered == false)
        {
            PowerPuzzleTile tileRef = other.gameObject.GetComponentInParent<PowerPuzzleTile>();
            if (tileRef.isPowered || tileRef.isPowerNode)
            {
                isPowered = true;
                ToggleConnectionMaterials(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PowerPuzzleTile>() && !isPowerNode)
        {
            isPowered = false;
            ToggleConnectionMaterials(false);
        }
    }

    private void ToggleConnectionMaterials(bool _isPowered)
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

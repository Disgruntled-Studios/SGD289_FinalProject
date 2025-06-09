using UnityEngine;
using UnityEngine.InputSystem;

public class TileSelection : MonoBehaviour
{
    public GameObject selectedOBJ;
    public float xLimit = 10f;
    public float yLimit = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PowerPuzzleTile>())
        {
            selectedOBJ = other.gameObject;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PowerPuzzleTile>())
        {
            selectedOBJ = other.gameObject;
        }
    }
}

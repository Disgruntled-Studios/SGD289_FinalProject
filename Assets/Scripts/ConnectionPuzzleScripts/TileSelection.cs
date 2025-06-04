using UnityEngine;

public class TileSelection : MonoBehaviour
{
    public GameObject selectedOBJ;

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

    public void MoveSelection(float inputX, float inputY)
    {
        transform.position = new Vector3(transform.position.x + (inputX * 2 * Time.deltaTime), transform.position.y + (inputY * 2 * Time.deltaTime), 0);
    }
}

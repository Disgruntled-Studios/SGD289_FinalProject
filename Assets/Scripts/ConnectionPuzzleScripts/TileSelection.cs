using UnityEngine;

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

    public void MoveSelection(float inputX, float inputY)
    {
        if (transform.localPosition.x <= xLimit && Mathf.Sign(inputX) != -1 || transform.localPosition.x >= -xLimit && Mathf.Sign(inputX) == -1)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + (inputX * 2 * Time.deltaTime), transform.localPosition.y, 0);
        }

        if (transform.localPosition.y <= yLimit && Mathf.Sign(inputY) != -1 || transform.localPosition.y >= -yLimit && Mathf.Sign(inputY) == -1)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (inputY * 2 * Time.deltaTime), 0);
        }
    }
}

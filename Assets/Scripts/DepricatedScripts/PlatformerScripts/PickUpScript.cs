using UnityEngine;

public class PickUpScript : MonoBehaviour
{

    [SerializeField]
    private float xValue = 0f;
    [SerializeField]
    private float yValue = 45f;
    [SerializeField]
    private float zValue = 0f;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(xValue, yValue, zValue) * Time.deltaTime);
    }
}

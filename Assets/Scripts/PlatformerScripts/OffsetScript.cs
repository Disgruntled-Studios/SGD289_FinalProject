using UnityEngine;

public class OffSetScript : MonoBehaviour
{
    //allows objects to be offset from another object.

    [SerializeField]
    private GameObject targetObj;

    [SerializeField]
    private Vector3 objOffset;
    
    void Start()
    {
        try
        {
            objOffset = transform.position - targetObj.transform.position;
        }
        catch
        {
            print("problem with finding targetObj in " + this.gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetObj.transform.position + objOffset;
    }
}

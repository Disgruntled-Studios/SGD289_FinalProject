using UnityEngine;
using System.Collections;

public class Gears : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //want it to spin a gear, stop and make a click noise, and then spin a gear again

    private Transform currentTransform;
    private Vector3 targetTransform;
    [SerializeField]
    private float waitTime = 2f;

    [SerializeField]
    private bool rotateFromStart = false;

    [SerializeField]
        private float speed = 15f;

    private bool rotating;
    void Start()
    {
        currentTransform = this.gameObject.transform;
        targetTransform = new Vector3(currentTransform.rotation.x, currentTransform.rotation.y, currentTransform.rotation.z - 90);
        print("currentTransform = " + currentTransform + "and targetTransform = " + targetTransform);

        if (rotateFromStart == true)
        {
            rotating = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(rotating == true)
        {
            //rotates cubes
            this.gameObject.transform.Rotate(0, 0, speed * Time.deltaTime);

            if(this.gameObject.transform.rotation.z < targetTransform.z)
            {
                print("rotating = false");
                rotating = false;
                StartCoroutine("Wait");  
            }

        }
    }

    IEnumerator Wait()
    {
        Debug.Log("starting wait coroutine");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("ending wait coroutine");
        currentTransform = this.gameObject.transform;
        targetTransform = new Vector3(currentTransform.rotation.x, currentTransform.rotation.y, currentTransform.rotation.z + 90);
        rotating = true;
    }

    //called from anim.
    void StartRotating()
    {
        rotating = true;
    }
}

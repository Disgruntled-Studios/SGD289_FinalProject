using Unity.VisualScripting;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        CastLaser();
    }

    void CastLaser()
    {
        RaycastHit hit;

        //Shoot a ray forward to see if there is an object to hit.
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                //If we hit something and it has a collider set the lasers endpoint to that raycast hitpoint
                lr.SetPosition(1, new Vector3(0, 0, hit.distance));
                return;
            }
        }
        //if we hit nothing push the endpoint of the laser far out.
        lr.SetPosition(1, new Vector3(0, 0, 5000));
    }
}

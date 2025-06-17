using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

public class GunScript : MonoBehaviour
{
    //This script is referenced by PlatformPlayerMode Script in Attack function

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform startPointTransform;

    [SerializeField] private LineRenderer _lr;

    //adjust height of shooting before doing anims
    [SerializeField]
    public bool adjustHeight = false;
    [SerializeField]
    private float heightAdj = 0.3f;

    public void Shoot()
    {
        if (adjustHeight == true)
        {
            Vector3 bulletHeight = new Vector3(startPointTransform.position.x, startPointTransform.position.y +heightAdj, startPointTransform.position.z);
            Instantiate(bullet, bulletHeight, Quaternion.identity);
        }
        else
        {
            Instantiate(bullet, startPointTransform.position, Quaternion.identity);
        }
    }

    
    // From BJ: if you want to turn the laser on or off in your level
    public void ToggleLineRenderer(bool isActive)
    {
        _lr.enabled = isActive;
    }
}

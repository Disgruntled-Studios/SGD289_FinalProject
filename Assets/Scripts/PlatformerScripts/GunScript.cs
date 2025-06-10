using UnityEngine;
using UnityEngine.ProBuilder;

public class GunScript : MonoBehaviour
{
    //This script is referenced by PlatformPlayerMode Script in Attack function

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform startPointTransform;

    [SerializeField] private LineRenderer _lr;

    public void Shoot()
    {
        Instantiate(bullet, startPointTransform.position, Quaternion.identity);
    }

    
    // From BJ: if you want to turn the laser on or off in your level
    public void ToggleLineRenderer(bool isActive)
    {
        _lr.enabled = isActive;
    }
}

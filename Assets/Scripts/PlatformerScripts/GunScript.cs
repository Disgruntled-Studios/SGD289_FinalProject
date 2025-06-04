using UnityEngine;
using UnityEngine.ProBuilder;

public class GunScript : MonoBehaviour
{
    //This script is referenced by PlatformPlayerMode Script in Attack function

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform startPointTransform;

    public void Shoot()
    {
        Instantiate(bullet, startPointTransform.position, Quaternion.identity);
    }


}

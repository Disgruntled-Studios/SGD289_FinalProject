using UnityEngine;
using UnityEngine.ProBuilder;

public class GunScript : MonoBehaviour
{

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform startPointTransform;

    public void Shoot()
    {
        Instantiate(bullet, startPointTransform.position, Quaternion.identity);
    }


}

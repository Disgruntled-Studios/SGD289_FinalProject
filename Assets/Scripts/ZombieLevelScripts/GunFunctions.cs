using UnityEngine;
using UnityEngine.InputSystem;

public class GunFunctions : MonoBehaviour
{

    [SerializeField] private GameObject gunModel;
    [SerializeField] private Transform gunBarrelTransform;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float damageAmount;
    [SerializeField] private LineRenderer lr;
    bool isAiming;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isAiming)
        {
            gunModel.SetActive(true);
        }
        else
        {
            gunModel.SetActive(false);
        }
    }

    public void Shoot()
    {
        if (isAiming)
        {
                Debug.DrawLine(gunBarrelTransform.position, gunBarrelTransform.forward * 50);
            //Play SFX 

            //Play VFX

            //Shoot a ray to see if a monster is going to get hit.
            RaycastHit hit;

            if (Physics.Raycast(gunBarrelTransform.position, lr.GetPosition(1), out hit, enemyLayerMask))
            {
                Debug.Log("Enemy " + hit.collider.transform.gameObject.name + " Found");
                //Affect enemies health.
            }
        }
    }


    public void StartGunAim()
    {
        isAiming = true;
    }

    public void EndGunAim()
    {
        isAiming = false;
    }
}

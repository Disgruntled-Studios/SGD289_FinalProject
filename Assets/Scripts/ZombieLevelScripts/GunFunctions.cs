using UnityEngine;
using UnityEngine.InputSystem;

public class GunFunctions : MonoBehaviour
{

    [SerializeField] private GameObject gunModel;
    [SerializeField] private Transform gunBarrelTransform;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float damageAmount;
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
            //Play SFX 

            //Play VFX

            //Shoot a ray to see if a monster is going to get hit.
            RaycastHit hit;

            if (Physics.Raycast(gunBarrelTransform.position, Vector3.forward, out hit, 500, enemyLayerMask))
            {
                Debug.Log("Enemy " + hit.collider.transform.gameObject.name + " Found");
                //Affect enemies health.
            }
        }
    }


    public void StartGunAim(InputAction.CallbackContext context)
    {
        isAiming = true;
    }

    public void EndGunAim(InputAction.CallbackContext context)
    {
        isAiming = false;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerGumFunctions : MonoBehaviour
{

    [SerializeField] private GameObject gunModel;
    [SerializeField] private Transform gunBarrelTransform;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float damageAmount = 50f;
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
            //Debug.DrawLine(gunBarrelTransform.position, gunBarrelTransform.forward * 50);
            //Debug.DrawRay(gunBarrelTransform.position, gunBarrelTransform.forward, Color.green ,3f);
            //Debug.Log("Shot fired");
            //Play SFX 

            //Play VFX
            
            //Shoot a ray to see if a monster is going to get hit.
            RaycastHit hit;

            if (Physics.Raycast(gunBarrelTransform.position, gunBarrelTransform.forward, out hit, 100f, enemyLayerMask))
            {
                Debug.Log("hit " + hit.collider.transform.gameObject.name);
                //hit.transform.gameObject.SetActive(false);
                //Affect enemies health.
                if (hit.transform.gameObject.GetComponent<EnemyBehavior>())
                {
                    hit.transform.gameObject.GetComponent<EnemyBehavior>().health.Damage(damageAmount);
                    Debug.Log(hit.transform.gameObject.GetComponent<EnemyBehavior>().health.CurrentHealth);
                }
                // BJ NOTE: Raycast may hit hands or eyes which do not have enemybehavior component. May need to check against component in parent as well
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

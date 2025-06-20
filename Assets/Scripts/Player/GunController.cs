using System.Collections;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GunController : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private Transform laserStart;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _damageAmount = 50f;
    [SerializeField, Range(1,10)] float reloadSpeed = 5f;
    [SerializeField] int maxMagLimit = 12;
    [SerializeField] int currentAmmoMagAmt = 0;

    [Header("Laser")]
    [SerializeField] private LineRenderer _lr;

    private bool _isAiming;
    public bool IsAiming => _isAiming;
    
    private bool _isReloading;
    public bool IsReloading => _isReloading;
    
    private void Start()
    {
        if (_lr != null)
        {
            _lr.enabled = false;
            _lr.SetPosition(0, new Vector3(0, 0, 0));
        }
        
        StartCoroutine(ReloadGun());
    }

    private void Update()
    {
        _gunModel.SetActive(_isAiming);

        if (_isAiming && _lr != null)
        {
            HandleLaser();
        }
        else if (_lr != null)
        {
            _lr.enabled = false;
        }
    }

    public void StartGunAim()
    {
        _isAiming = true;
    }

    public void EndGunAim()
    {
        _isAiming = false;
    }

    public void HandleLaser()
    {
        UpdateTankLaser();
    }

    public void HandleShoot()
    {
        ShootForTank();
    }

    private void UpdateTankLaser()
    {
        _lr.enabled = true;
        RaycastHit hit;
        //Shoot a ray forward to see if there is an object to hit.
        if (Physics.Raycast(laserStart.position, laserStart.forward, out hit))
        {
            if (hit.collider && !hit.collider.isTrigger)
            {
                //If we hit something and it has a collider set the lasers endpoint to that raycast hitpoint
                _lr.SetPosition(1, new Vector3(0, 0, hit.distance));
                return;
            }
        }
        //if we hit nothing push the endpoint of the laser far out.
        _lr.SetPosition(1, new Vector3(0, 0, 5000));

    }

    public void ShootForTank()
    {
        if (_isAiming && currentAmmoMagAmt > 0 && !_isReloading)
        {
            Debug.Log("Shooting");
            currentAmmoMagAmt--;
            UIManager.Instance.UpdateAmmoText(currentAmmoMagAmt, maxMagLimit);
            //Play SFX 
            //Play VFX

            //Shoot a ray to see if a monster is going to get hit.
            RaycastHit hit;

            if (Physics.Raycast(laserStart.position, laserStart.forward, out hit, 100f, _enemyLayer))
            {
                Debug.Log("hit " + hit.collider.transform.gameObject.name);
                //hit.transform.gameObject.SetActive(false);
                //Affect enemies health.
                if (hit.transform.gameObject.GetComponent<EnemyBehavior>())
                {
                    hit.transform.gameObject.GetComponent<EnemyBehavior>().health.Damage(_damageAmount);
                    //Debug.Log(hit.transform.gameObject.GetComponent<EnemyBehavior>().health.CurrentHealth);
                }
                else if (hit.transform.gameObject.GetComponentInParent<EnemyBehavior>())
                {
                   hit.transform.gameObject.GetComponentInParent<EnemyBehavior>().health.Damage(_damageAmount);
                    //Debug.Log(hit.transform.gameObject.GetComponent<EnemyBehavior>().health.CurrentHealth); 
                }
                // BJ NOTE: Raycast may hit hands or eyes which do not have enemybehavior component. May need to check against component in parent as well
            }
        }
        else if (_isAiming)
        {
            StartCoroutine(ReloadGun());
        }
    }

    IEnumerator ReloadGun()
    {
        //Debug.Log("Is Reloading");
        _isReloading = true;
        UIManager.Instance.ShowReloading();

        //call animation to reload

        //Call reload SFX
        yield return new WaitForSeconds(reloadSpeed);
        currentAmmoMagAmt = maxMagLimit;
        UIManager.Instance.UpdateAmmoText(currentAmmoMagAmt, maxMagLimit);
        Debug.Log("Is Reloaded");
        _isReloading = false;
    }

}

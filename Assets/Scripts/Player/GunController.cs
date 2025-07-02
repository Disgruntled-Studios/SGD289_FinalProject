using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GunController : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animationController;

    [Header("Gun")]
    [SerializeField] private Transform gunPoint;
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private Transform laserStart;
    [SerializeField] private LayerMask _shootableLayers;
    [SerializeField] private float _damageAmount = 50f;
    [SerializeField, Range(1,10)] float reloadSpeed = 5f;
    [SerializeField] int maxMagLimit = 12;
    [SerializeField] int currentAmmoMagAmt = 0;
    [SerializeField] string _gunItemName;
    [SerializeField] string gunShotSFX;

    [Header("Laser")]
    [SerializeField] private LineRenderer _lr;

    private bool _isAiming;
    public bool IsAiming => _isAiming;
    
    public bool _canShoot;
    public bool HasGun { get; set; }
    private SoundComponent soundComponent;

    private void Start()
    {
        soundComponent = GetComponent<SoundComponent>();
        if (_lr)
        {
            _lr.enabled = false;
            _lr.SetPosition(0, new Vector3(0, 0, 0));
        }

        _animationController = GetComponentInParent<PlayerAnimationController>();
        _canShoot = true;
        //StartCoroutine(ReloadGun());
        transform.position = gunPoint.position;
        transform.rotation = gunPoint.rotation;
    }

    private void Update()
    {
        if (!HasGun) return;
        if (_isAiming && _lr)
        {
            HandleLaser();
        }
        else if (_lr)
        {
            _lr.enabled = false;
        }
        transform.position = gunPoint.position;
        transform.rotation = gunPoint.rotation;
    }

    public void StartGunAim()
    {
        if (!HasGun) return;
        
        _isAiming = true;
        _gunModel.SetActive(true);
    }

    public void EndGunAim()
    {
        if (!HasGun) return;
        
        _isAiming = false;
        _gunModel.SetActive(false);
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
                Debug.Log("object hit is " + hit.collider.name);
                _lr.SetPosition(1, new Vector3(0, 0, hit.distance));
                return;
            }
        }
        else
        {
            //if we hit nothing push the endpoint of the laser far out.
            _lr.SetPosition(1, new Vector3(0, 0, 5000));
        }

    }

    public void ShootForTank()
    {
        if (_isAiming && _canShoot)
        {
            Debug.Log("Shooting");
            _animationController.Shoot();
            soundComponent.PlaySFX(gunShotSFX);
            currentAmmoMagAmt--;
            //UIManager.Instance.UpdateAmmoText(currentAmmoMagAmt, maxMagLimit);
            //Play SFX 
            //Play VFX

            //Shoot a ray to see if a monster is going to get hit.
            RaycastHit hit;

            if (Physics.Raycast(laserStart.position, laserStart.forward, out hit, 100f, _shootableLayers))
            {
                //Debug.Log("hit " + hit.collider.transform.gameObject.name);
                EnemyBehavior enemyRef = hit.transform.gameObject.GetComponent<EnemyBehavior>();
                //hit.transform.gameObject.SetActive(false);
                //Affect enemies health.
                if (enemyRef == null)
                {
                    enemyRef = hit.transform.gameObject.GetComponentInParent<EnemyBehavior>();
                    //Debug.Log(hit.transform.gameObject.GetComponent<EnemyBehavior>().health.CurrentHealth);
                }

                if (hit.transform.gameObject.GetComponent<ShootableObject>())
                {
                    hit.transform.gameObject.GetComponent<ShootableObject>().OnShot();
                }

                if (enemyRef != null && !enemyRef.health.IsDead)
                {
                    enemyRef.health.Damage(_damageAmount);
                    Debug.Log(enemyRef.health.CurrentHealth + " health remaining " + enemyRef.name);
                }
                // BJ NOTE: Raycast may hit hands or eyes which do not have enemybehavior component. May need to check against component in parent as well
            }
            StartCoroutine(ShootDelay());
        }
    }


    
    public IEnumerator ShootDelay()
    {
        _canShoot = false;
        yield return new WaitForSeconds(.75f);
        _canShoot = true;
    }

}

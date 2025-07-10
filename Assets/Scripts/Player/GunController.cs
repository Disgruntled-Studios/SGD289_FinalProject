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
    [SerializeField] private LayerMask _shootableLayers; // Enemy and Shootable
    [SerializeField] private float _damageAmount = 50f;
    [SerializeField, Range(1,10)] float reloadSpeed = 5f;
    [SerializeField] int maxMagLimit = 12;
    [SerializeField] int currentAmmoMagAmt = 0;
    [SerializeField] string _gunItemName;
    [SerializeField] string gunShotSFX;

    [Header("Laser")]
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private Material redLaser;
    [SerializeField] private Gradient redLaserGradient;
    [SerializeField] private Gradient greenLaserGradient;
    [SerializeField] private Material greenLaser;

    private bool _isAiming;
    public bool IsAiming => _isAiming;
    
    public bool _canShoot;
    public bool HasGun { get; set; }
    private SoundComponent soundComponent;
    [HideInInspector]public ShootableObject closeObj;

    private const float MinVisualDistance = 5f;
    private const float MaxLaserDistance = 100f;


    private void Start()
    {
        soundComponent = GetComponent<SoundComponent>();
        if (_lr)
        {
            _lr.enabled = false;
            _lr.SetPosition(0, Vector3.zero);
        }

        _animationController = GetComponentInParent<PlayerAnimationController>();
        _canShoot = true;
        //StartCoroutine(ReloadGun());
        transform.position = gunPoint.position;
        transform.rotation = gunPoint.rotation;
        _lr.colorGradient = greenLaserGradient;
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

        if (Physics.Raycast(laserStart.position, laserStart.forward, out var hit, MaxLaserDistance))
        {
            var isShootable = (_shootableLayers.value & (1 << hit.collider.gameObject.layer)) != 0;

            _lr.material = isShootable ? redLaser : greenLaser;
            _lr.colorGradient = isShootable ? redLaserGradient : greenLaserGradient;

            var visualDistance = Mathf.Max(hit.distance, MinVisualDistance);
            _lr.SetPosition(1, new Vector3(0, 0, visualDistance));
        }
        else
        {
            _lr.material = greenLaser;
            _lr.colorGradient = greenLaserGradient;
            _lr.SetPosition(1, new Vector3(0, 0, MaxLaserDistance));
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
                else if (closeObj != null)
                {
                    closeObj.OnShot();
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

using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GunController : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animationController;
    [SerializeField] private PlayerController _playerController;

    [Header("Gun")]
    [SerializeField] private Transform gunPoint;
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private Transform laserStart;
    [SerializeField] private LayerMask _shootableLayers; // Enemy and Shootable
    [SerializeField] private float _damageAmount = 50f;
    [SerializeField] string _gunItemName;

    [Header("Laser")]
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private Material redLaser;
    [SerializeField] private Gradient redLaserGradient;
    [SerializeField] private Gradient greenLaserGradient;
    [SerializeField] private Material greenLaser;

    [SerializeField] private AudioSource _gunShot;

    private bool _isAiming;
    public bool IsAiming => _isAiming;
    
    public bool _canShoot;
    public bool HasGun { get; set; }
    [HideInInspector]public ShootableObject closeObj;
    
    private const float MaxLaserDistance = 100f;
    private const float AimRadius = 0.175f;
    
    private void Start()
    {
        if (_lr)
        {
            _lr.enabled = false;
            _lr.SetPosition(0, Vector3.zero);
        }

        _animationController = GetComponentInParent<PlayerAnimationController>();
        _canShoot = true;
        transform.position = gunPoint.position;
        transform.rotation = gunPoint.rotation;
        _lr.colorGradient = greenLaserGradient;
    }

    private void Update()
    {
        if (!HasGun || _playerController.IsCrouching) return;
        
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
        if (!HasGun || _playerController.IsCrouching) return;
        
        _isAiming = true;
        _gunModel.SetActive(true);
    }

    public void EndGunAim()
    {
        if (!HasGun || _playerController.IsCrouching) return;
        
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

        if (Physics.SphereCast(laserStart.position, AimRadius, laserStart.forward, out var hit, MaxLaserDistance, Physics.DefaultRaycastLayers))
        {
            var isShootable = (_shootableLayers.value & (1 << hit.collider.gameObject.layer)) != 0;

            _lr.material = isShootable ? redLaser : greenLaser;
            _lr.colorGradient = isShootable ? redLaserGradient : greenLaserGradient;

            _lr.SetPosition(1, new Vector3(0, 0, hit.distance));
        }
        else
        {
            _lr.material = greenLaser;
            _lr.colorGradient = greenLaserGradient;
            _lr.SetPosition(1, new Vector3(0, 0, MaxLaserDistance));
        }
    }

    private void ShootForTank()
    {
        if (!_isAiming || !_canShoot || _playerController.IsCrouching) return;

        _canShoot = false;
        _animationController.Shoot();
        _gunShot.PlayOneShot(_gunShot.clip);
        RumbleController.Instance.TriggerPresetRumble(RumblePreset.GunRecoil);

        if (Physics.SphereCast(laserStart.position, AimRadius, laserStart.forward, out var hit, MaxLaserDistance, Physics.DefaultRaycastLayers))
        {
            var hitObj = hit.collider.gameObject;

            var enemyRef = hitObj.GetComponent<EnemyBehavior>() ?? hitObj.GetComponentInParent<EnemyBehavior>();
            var shootable = hitObj.GetComponent<ShootableObject>();

            shootable?.OnShot();

            if (shootable == null && closeObj != null)
            {
                closeObj.OnShot();
            }

            if (enemyRef?.health?.IsDead == false)
            {
                enemyRef.health.Damage(_damageAmount);
            }
        }

        StartCoroutine(ShootDelay());
    }

    private IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(.75f);
        _canShoot = true;
    }

}

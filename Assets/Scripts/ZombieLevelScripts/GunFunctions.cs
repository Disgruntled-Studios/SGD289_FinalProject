using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunFunctions : MonoBehaviour
{
    [Header("Gun")] 
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _damageAmount = 50f;

    [Header("Laser")] 
    [SerializeField] private LineRenderer _lr;

    public bool isAiming;

    private void Start()
    {
        if (_lr != null)
        {
            _lr.enabled = false;
        }
    }

    private void Update()
    {
        _gunModel.SetActive(isAiming);

        if (isAiming && _lr != null)
        {
            UpdateLaser();
        }
        else if (_lr != null)
        {
            _lr.enabled = false;
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

    private void UpdateLaser()
    {
        _lr.enabled = true;
        _lr.SetPosition(0, _gunBarrel.localPosition);

        var cam = Camera.main;
        var centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 targetPoint;
        if (Physics.Raycast(centerRay, out var hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = centerRay.origin + centerRay.direction * 100f;
        }

        var direction = (targetPoint - cam.transform.position).normalized;
        var laserEnd = _gunBarrel.position + direction * 100f;

        if (Physics.Linecast(_gunBarrel.position, laserEnd, out var finalHit))
        {
            _lr.SetPosition(1, finalHit.point);
        }
        else
        {
            _lr.SetPosition(1, laserEnd);
        }
    }

    public void Shoot()
    {
        if (!isAiming) return;

        var cam = Camera.main;
        var centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 targetPoint;
        if (Physics.Raycast(centerRay, out var hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = centerRay.origin + centerRay.direction * 100f;
        }

        var direction = (targetPoint - cam.transform.position).normalized;
        var laserEnd = _gunBarrel.position + direction * 100f;

        if (Physics.Linecast(_gunBarrel.position, laserEnd, out var finalHit, _enemyLayer))
        {
            var enemy = finalHit.collider.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.health.Damage(_damageAmount);
            }
            else if (finalHit.collider.transform.parent != null)
            {
                enemy = finalHit.collider.transform.parent.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    enemy.health.Damage(_damageAmount);
                }
            }
        }
    }

}

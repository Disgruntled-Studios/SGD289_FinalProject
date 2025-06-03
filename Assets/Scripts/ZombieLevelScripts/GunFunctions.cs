using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunFunctions : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private Transform laserStart;
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
            UpdateTankLaser();
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
        if (GameManager.Instance.CurrentWorld == World.Tank)
        {
            return;
        }
        _lr.enabled = true;
        _lr.SetPosition(0, laserStart.localPosition);

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
        var laserEnd = laserStart.position + direction * 100f;

        if (Physics.Linecast(laserStart.position, laserEnd, out var finalHit))
        {
            _lr.SetPosition(1, finalHit.point);
        }
        else
        {
            _lr.SetPosition(1, laserEnd);
        }
    }

    private void UpdateTankLaser()
    {
        _lr.enabled = true;
        RaycastHit hit;
        //Shoot a ray forward to see if there is an object to hit.
        if (Physics.Raycast(transform.position, transform.forward, out hit))
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

    public void Shoot()
    {
        if (!isAiming) return;

        var cam = Camera.main;
        var centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 targetPoint;
        if (Physics.Raycast(centerRay, out var hit, 100f))
        {
            Debug.Log("hit!");
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = centerRay.origin + centerRay.direction * 100f;
        }

        var direction = (targetPoint - cam.transform.position).normalized;
        var laserEnd = laserStart.position + direction * 100f;

        if (Physics.Linecast(laserStart.position, laserEnd, out var finalHit, _enemyLayer))
        {
            Debug.Log("Enemy detected!");
            Debug.DrawLine(laserStart.position, laserEnd, Color.green, 2f);
            var enemy = finalHit.collider.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.health.Damage(_damageAmount);
                Debug.Log("Enemy " + enemy.name + " hit!");
            }
            else if (finalHit.collider.transform.parent != null)
            {
                enemy = finalHit.collider.transform.parent.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    enemy.health.Damage(_damageAmount);
                    Debug.Log("Enemy " + enemy.name + " hit!");
                }
            }
        }
    }

    public void ShootForTank()
    {
        if (isAiming)
        {
            Debug.Log("Shooting");

            //Debug.DrawLine(gunBarrelTransform.position, gunBarrelTransform.forward * 50);
            //Debug.DrawRay(gunBarrelTransform.position, gunBarrelTransform.forward, Color.green ,3f);
            //Debug.Log("Shot fired");
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
                    Debug.Log(hit.transform.gameObject.GetComponent<EnemyBehavior>().health.CurrentHealth);
                }
                // BJ NOTE: Raycast may hit hands or eyes which do not have enemybehavior component. May need to check against component in parent as well
            }
        }
    }

}

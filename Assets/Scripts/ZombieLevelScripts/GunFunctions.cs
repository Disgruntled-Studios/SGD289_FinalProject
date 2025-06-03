using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunFunctions : MonoBehaviour
{

    [SerializeField] private GameObject gunModel;
    [SerializeField] private Transform gunBarrelTransform;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float damageAmount = 50f;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private LaserSight _laserSight;
    public bool isAiming;

    // Update is called once per frame
    private void Update()
    {
        gunModel.SetActive(isAiming);
    }

    public void UpdateLaser()
    {
        _laserSight.UpdateLaser(_laserSight.transform, isAiming);
    }
    
    public void Shoot()
    {
        if (!isAiming) return;

        var cameraTransform = Camera.main.transform;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, 100f, enemyLayerMask))
        {
            var enemy = hit.collider.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.health.Damage(damageAmount);
            }
            else if (hit.collider.transform.parent != null)
            {
                enemy = hit.collider.transform.parent.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    enemy.health.Damage(damageAmount);
                }
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

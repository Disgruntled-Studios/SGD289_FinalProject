using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FPSGunController : MonoBehaviour
{
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private Transform _laserStartPos;

    [SerializeField] private GameObject _bulletPrefab;

    private bool _isAiming;

    private void OnEnable()
    {
        _rightHand.SetActive(true);
        _leftHand.SetActive(true);
        _gunModel.SetActive(true);
    }
    
    public void StartGunAim()
    {
        _isAiming = true;
    }

    public void EndGunAim()
    {
        _isAiming = false;
    }

    public void ShootRaycast()
    {
        var origin = _laserStartPos.position;
        var direction = _laserStartPos.rotation * Vector3.forward;

        if (Physics.Raycast(origin, direction, out var hit, 100f, _enemyLayer))
        {
            Debug.Log($"Hit: {hit.collider.gameObject.name}");
        }

    }

    public void Shoot()
    {
        var bullet = Instantiate(_bulletPrefab, _laserStartPos.position, _laserStartPos.rotation);
        var rb = bullet.GetComponent<Rigidbody>();

        rb.AddForce(bullet.transform.forward * 50f, ForceMode.Impulse);
    }
}

using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class FPSGunController : MonoBehaviour
{
    [Header("Hands")]
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _leftHand;
    
    [Header("Gun")]
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private GameObject _gunHandle;
    [SerializeField] private GameObject _gunBarrel;
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private Transform _barrelEnd;

    [Header("Materials")] 
    [SerializeField] private Material _blueMat;
    [SerializeField] private Material _greenMat;
    [SerializeField] private Material _redMat;
    
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _bulletPrefab;

    private bool _isAiming;

    private void OnEnable()
    {
        _rightHand.SetActive(true);
        _leftHand.SetActive(true);
        _gunModel.SetActive(true);

        _lr.enabled = false;
    }
    
    public void StartGunAim()
    {
        _isAiming = true;
    }

    public void EndGunAim()
    {
        _isAiming = false;
    }

    public void Shoot()
    {
        var bullet = Instantiate(_bulletPrefab, _barrelEnd.position, _barrelEnd.rotation);
        var bulletController = bullet.GetComponent<FPSBulletController>();

        bulletController.Initialize();
    }
}

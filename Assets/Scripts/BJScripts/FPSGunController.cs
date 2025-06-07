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
    [SerializeField] private Transform _barrelEnd;

    [Header("Materials")] 
    [SerializeField] private Material[] _materials;
    private int _matIndex;
    
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _bulletPrefab;

    private bool _isAiming;

    public Material CurrentMaterial => _materials[_matIndex];
    
    private void OnEnable()
    {
        _rightHand.SetActive(true);
        _leftHand.SetActive(true);
        _gunModel.SetActive(true);

        _gunHandle.GetComponent<MeshRenderer>().material = CurrentMaterial;
        _gunBarrel.GetComponent<MeshRenderer>().material = CurrentMaterial;
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

        bulletController.InitializeAndFire(CurrentMaterial);
    }

    public void ChangeColor()
    {
        _matIndex = (_matIndex + 1) % _materials.Length;
        _gunBarrel.GetComponent<MeshRenderer>().material = CurrentMaterial;
        _gunHandle.GetComponent<MeshRenderer>().material = CurrentMaterial;
    }
}

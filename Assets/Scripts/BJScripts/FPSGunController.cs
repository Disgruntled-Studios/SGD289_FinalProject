using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class FPSGunController : MonoBehaviour
{
    [Header("Hands")] [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _leftHand;

    [Header("Gun")] 
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private Transform _barrelEnd;
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private GameObject _collider;
    
    [Header("Materials")] 
    [SerializeField] private Material[] _materials;
    private int _matIndex;

    [Header("Recoil")] 
    [SerializeField] private float _recoilZRotation = -10f;
    [SerializeField] private float _recoilSpeedUp = 25f;
    [SerializeField] private float _recoilSpeedDown = 10f;
    private float _currentZRotation;
    private bool _isRecoiling;
    
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _bulletPrefab;

    private bool _isAiming;

    private Material CurrentMaterial => _materials[_matIndex];

    private void Awake()
    {
        _currentZRotation = transform.rotation.z;
    }

    public void ToggleGunAndHands(bool isActive)
    {
        _rightHand.SetActive(isActive);
        _leftHand.SetActive(isActive);
        _gunModel.SetActive(isActive);
        _collider.SetActive(isActive);
        _lr.enabled = isActive;
    }

    private void Update()
    {
        if (_isRecoiling)
        {
            ApplyRecoilEffect(GameManager.Instance.IsBulletTime);
        }
        else
        {
            ResetRecoilEffect(GameManager.Instance.IsBulletTime);
        }

        var currentEuler = transform.localEulerAngles;
        currentEuler.z = _currentZRotation;
        transform.localEulerAngles = currentEuler;
    }

    private void ApplyRecoilEffect(bool isBulletTime)
    {
        _currentZRotation = isBulletTime ? Mathf.Lerp(_currentZRotation, _recoilZRotation, _recoilSpeedUp * Time.unscaledDeltaTime) : Mathf.Lerp(_currentZRotation, _recoilZRotation, _recoilSpeedUp * Time.deltaTime);

        if (Mathf.Abs(_currentZRotation - _recoilZRotation) < 0.1f)
        {
            _isRecoiling = false;
        }
    }

    private void ResetRecoilEffect(bool isBulletTime)
    {
        _currentZRotation = isBulletTime ? Mathf.Lerp(_currentZRotation, 0f, _recoilSpeedDown * Time.unscaledDeltaTime) : Mathf.Lerp(_currentZRotation, 0f, _recoilSpeedDown * Time.deltaTime);
    }
    
    public void StartGunAim()
    {
        Time.timeScale = 0.75f;
    }

    public void EndGunAim()
    {
        Time.timeScale = 1f;
    }

    public void Shoot()
    {
        var bullet = Instantiate(_bulletPrefab, _barrelEnd.position, _barrelEnd.rotation);
        var bulletController = bullet.GetComponent<FPSBulletController>();

        bulletController.InitializeAndFire(CurrentMaterial);

        _isRecoiling = true;
    }

    public void ChangeColor()
    {
        _matIndex = (_matIndex + 1) % _materials.Length;
        _lr.material = _materials[_matIndex];
    }
}

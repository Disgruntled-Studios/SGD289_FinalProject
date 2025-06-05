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
    [SerializeField] private Transform _cameraPivot;

    private bool _isAiming;
    public bool ShouldShoot { get; set; }

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

    private void LateUpdate()
    {
        if (_isAiming && ShouldShoot)
        {
            Shoot();
            ShouldShoot = false;
        }
    }

    public void Shoot()
    {
        var origin = _cameraPivot.position;
        var direction = _cameraPivot.forward;

        const float MaxDistance = 100f;
        var hitPoint = origin + direction * MaxDistance;

        if (Physics.Raycast(origin, direction, out var hit, MaxDistance, _enemyLayer))
        {
            hitPoint = hit.point;
        }

        if (_lr != null)
        {
            StartCoroutine(ShowShotLine(_laserStartPos.position, hitPoint));
        }
    }

    private IEnumerator ShowShotLine(Vector3 start, Vector3 end)
    {
        _lr.SetPosition(0, start);
        _lr.SetPosition(1, end);
        _lr.enabled = true;

        yield return new WaitForSeconds(1f);

        _lr.enabled = false;
    }
}

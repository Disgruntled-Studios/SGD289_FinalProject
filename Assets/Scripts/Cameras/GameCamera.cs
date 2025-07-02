using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private string _cameraId;
    public string CameraID => _cameraId;

    [SerializeField] private bool _isCCTV;
    public bool IsCCTV => _isCCTV;

    [SerializeField] private bool _shouldSetTarget = true;
    
    private CinemachineCamera _vCam;

    [SerializeField] private GameObject _triggerZone;

    private Quaternion _startingRotation;

    private void Start()
    {
        _vCam = GetComponent<CinemachineCamera>();

        if (!_vCam)
        {
            Debug.Log("Must attach GameCamera script");
            return;
        }

        var volume = GetComponent<Volume>();
        CameraManager.Instance.RegisterCamera(_cameraId, _vCam, _isCCTV, _shouldSetTarget, volume);

        _startingRotation = transform.rotation;
    }

    private void OnDestroy()
    {
        if (CameraManager.Instance)
        {
            CameraManager.Instance.UnregisterCamera(_cameraId);
        }
    }

    public void ResetRotation()
    {
        transform.rotation = _startingRotation;
    }
}

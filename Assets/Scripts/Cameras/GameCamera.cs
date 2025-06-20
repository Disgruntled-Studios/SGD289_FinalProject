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
    
    private CinemachineCamera _vCam;

    private void Start()
    {
        _vCam = GetComponent<CinemachineCamera>();

        if (!_vCam)
        {
            Debug.Log("Must attach GameCamera script");
            return;
        }

        var volume = GetComponent<Volume>();
        CameraManager.Instance.RegisterCamera(_cameraId, _vCam, _isCCTV, volume);
    }

    private void OnDestroy()
    {
        if (CameraManager.Instance)
        {
            CameraManager.Instance.UnregisterCamera(_cameraId);
        }
    }
}

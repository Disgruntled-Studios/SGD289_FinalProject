using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private string _cameraId;
    public string CameraID
    {
        get
        {
            return _cameraId;
        }
    }
    private CinemachineCamera _vCam;

    private void Start()
    {
        _vCam = GetComponent<CinemachineCamera>();

        if (!_vCam)
        {
            Debug.Log("Must attach GameCamera script");
            return;
        }

        if (CameraManager.Instance)
        {
            CameraManager.Instance.RegisterCamera(_cameraId, _vCam);
        }
    }

    private void OnDestroy()
    {
        if (CameraManager.Instance)
        {
            CameraManager.Instance.UnregisterCamera(_cameraId);
        }
    }
}

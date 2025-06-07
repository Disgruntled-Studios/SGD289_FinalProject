using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Serialization;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private readonly Dictionary<string, CinemachineCamera> _cameraRegistry = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterCamera(string id, CinemachineCamera cam)
    {
        _cameraRegistry.TryAdd(id, cam);
    }

    public void UnregisterCamera(string id)
    {
        if (_cameraRegistry.ContainsKey(id))
        {
            _cameraRegistry.Remove(id);
        }
    }

    public bool TrySwitchToCamera(string id)
    {
        if (_cameraRegistry.TryGetValue(id, out var cam))
        {
            foreach (var vCam in _cameraRegistry.Values)
            {
                vCam.Priority = 0;
            }

            cam.Priority = 10;
            return true;
        }

        return false;
    }

    public bool TrySetCameraTarget(string cameraId, Transform target)
    {
        if (_cameraRegistry.TryGetValue(cameraId, out var vCam))
        {
            vCam.Follow = target;
            return true;
        }

        return false;
    }

    public bool HasCamera(string id)
    {
        return _cameraRegistry.ContainsKey(id);
    }
}

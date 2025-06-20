using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Serialization;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private readonly Dictionary<string, CameraInfo> _cameraRegistry = new();

    [SerializeField] private CinemachineBrain _brain;
    [SerializeField] private Volume _cctvVolume;

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

    private void Start()
    {
        TrySwitchToCamera("TPCAM");
    }

    public void RegisterCamera(string id, CinemachineCamera cam, bool isCctv, Volume cctvVolume = null)
    {
        var camInfo = new CameraInfo(cam, isCctv, cctvVolume);
        _cameraRegistry.TryAdd(id, camInfo);
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
        if (_cameraRegistry.TryGetValue(id, out var targetInfo))
        {
            foreach (var camInfo in _cameraRegistry.Values)
            {
                camInfo.VCam.Priority = 0;
                if (camInfo.CCTVVolume)
                {
                    camInfo.CCTVVolume.enabled = false;
                }
            }

            targetInfo.VCam.Priority = 10;
            if (targetInfo.CCTVVolume)
            {
                targetInfo.CCTVVolume.enabled = true;
            }

            EnableCctvFilter(targetInfo.IsCCTV);
            return true;
        }

        return false;
    }

    public bool TrySetCameraTarget(string cameraId, Transform target)
    {
        if (_cameraRegistry.TryGetValue(cameraId, out var camInfo))
        {
            camInfo.VCam.Follow = target;
            camInfo.VCam.LookAt = target;
            return true;
        }
        
        return false;
    }

    private void EnableCctvFilter(bool enable)
    {
        if (_cctvVolume)
        {
            _cctvVolume.enabled = enable;
        }
    }

    public void SetBlend(string style, float duration)
    {
        if (_brain)
        {
            _brain.DefaultBlend.Style = style switch
            {
                "EaseInOut" => CinemachineBlendDefinition.Styles.EaseInOut,
                "Cut" => CinemachineBlendDefinition.Styles.Cut,
                _ => CinemachineBlendDefinition.Styles.EaseInOut
            };

            _brain.DefaultBlend.Time = duration;
        }
    }
}

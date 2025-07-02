using System.Collections.Generic;
using System.Linq;
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

    private readonly List<string> _activeTriggerIds = new();
    private string _currentTriggerId;

    private const string ThirdPersonId = "TPCAM";
    
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

    private void Update()
    {
        if (_activeTriggerIds.Count == 0)
        {
            if (_currentTriggerId != ThirdPersonId)
            {
                _currentTriggerId = ThirdPersonId;
                TrySwitchToCamera(ThirdPersonId);
                TrySetCameraTarget(ThirdPersonId, GameManager.Instance.CameraTarget);
            }

            return;
        }

        var latestId = _activeTriggerIds[^1];

        if (_currentTriggerId != latestId)
        {
            _currentTriggerId = latestId;
            TrySwitchToCamera(latestId);
            TrySetCameraTarget(latestId, GameManager.Instance.CameraTarget);
        }
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

    public void RegisterActiveTrigger(string triggerId)
    {
        if (!_activeTriggerIds.Contains(triggerId))
        {
            _activeTriggerIds.Add(triggerId);
        }
    }

    public void UnregisterActiveTrigger(string triggerId)
    {
        if (_activeTriggerIds.Contains(triggerId))
        {
            _activeTriggerIds.Remove(triggerId);
        }
    }
}

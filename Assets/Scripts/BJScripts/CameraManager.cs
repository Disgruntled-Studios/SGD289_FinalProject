using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Serialization;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCameraBase _hubCamera;
    [SerializeField] private CinemachineVirtualCameraBase _tankCamera;

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

    public void SwitchTo(World mode)
    {
        if (_hubCamera)
        {
            _hubCamera.Priority = (mode == World.Hub) ? 10 : 0;
        }

        if (_tankCamera)
        {
            _tankCamera.Priority = (mode == World.Tank) ? 10 : 0;
            GetComponent<CinemachineBrain>().DefaultBlend.Style = 0;
        }
    }
}

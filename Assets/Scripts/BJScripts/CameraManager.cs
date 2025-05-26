using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCameraBase _hubCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SwitchTo(World mode)
    {
        _hubCamera.Priority = (mode == World.Hub) ? 10 : 0;
    }
}

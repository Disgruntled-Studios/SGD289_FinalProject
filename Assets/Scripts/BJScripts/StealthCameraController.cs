using Unity.Cinemachine;
using UnityEngine;

public class StealthCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase _centerCam;
    [SerializeField] private CinemachineVirtualCameraBase _rightCam;
    [SerializeField] private CinemachineVirtualCameraBase _leftCam;

    private void Start()
    {
        SwitchToCenterCam();
    }

    public void SwitchToCenterCam()
    {
        _centerCam.Priority = 10;
        _rightCam.Priority = 0;
        _leftCam.Priority = 0;
    }

    public void SwitchToRightCam()
    {
        _centerCam.Priority = 0;
        _rightCam.Priority = 10;
        _leftCam.Priority = 0;
    }

    public void SwitchToLeftCam()
    {
        _centerCam.Priority = 0;
        _rightCam.Priority = 0;
        _leftCam.Priority = 10;
    }
}

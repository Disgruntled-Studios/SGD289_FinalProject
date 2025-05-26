using System;
using UnityEngine;

public class CamTrigger : MonoBehaviour
{
    public static bool _isRightActive;
    public static bool _isLeftActive;

    [SerializeField] private StealthCameraController _scc;

    [SerializeField] private bool _isRightTrigger;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_isRightTrigger && _isRightActive)
        {
            _scc.SwitchToCenterCam();
            _isRightActive = false;
        }
        else if (_isRightTrigger && !_isRightActive)
        {
            _scc.SwitchToRightCam();
            _isRightActive = true;
        }
        else if (!_isRightTrigger && _isLeftActive)
        {
            _scc.SwitchToCenterCam();
            _isLeftActive = false;
        }
        else if (!_isRightTrigger && !_isLeftActive)
        {
            _scc.SwitchToLeftCam();
            _isLeftActive = true;
        }
    }
}

using UnityEngine;

public class FPSGunController : MonoBehaviour
{
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private LineRenderer _lr;

    private bool _isAiming;

    private void OnEnable()
    {
        _gunModel.SetActive(true);
    }
    
    private void Update()
    {
        _lr.enabled = _isAiming;
    }
    
    public void StartGunAim()
    {
        _isAiming = true;
    }

    public void EndGunAim()
    {
        _isAiming = false;
    }

    public void Shoot()
    {
        Debug.Log("Shoot");
        return;
    }
}

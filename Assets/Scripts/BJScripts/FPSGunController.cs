using System.Collections;
using UnityEngine;

public class FPSGunController : MonoBehaviour
{
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] private GameObject _gunModel;
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private LayerMask _enemyLayer;

    private bool _isAiming;

    private void OnEnable()
    {
        _rightHand.SetActive(true);
        _leftHand.SetActive(true);
        _gunModel.SetActive(true);
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
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var endPoint = ray.origin + ray.direction * 100f;

        if (Physics.Raycast(ray, out var hit, 100f, _enemyLayer))
        {
            Debug.Log($"Hit: {hit.collider.name}");
        }

        if (_lr != null)
        {
            StartCoroutine(ShowShotLine(ray.origin, endPoint));
        }
    }

    private IEnumerator ShowShotLine(Vector3 start, Vector3 end)
    {
        _lr.SetPosition(0, start);
        _lr.SetPosition(1, end);
        _lr.enabled = true;

        yield return new WaitForSeconds(10f);

        _lr.enabled = false;
    }
}

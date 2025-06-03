using Unity.VisualScripting;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void UpdateLaser(Transform laserStart, bool isAiming)
    {
        if (!isAiming)
        {
            lr.enabled = false;
            return;
        }

        lr.enabled = true;

        var cam = Camera.main;
        var centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        lr.SetPosition(0, centerRay.origin);

        Vector3 targetPoint;
        if (Physics.Raycast(centerRay, out var hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = centerRay.origin + centerRay.direction * 100f;
        }

        lr.SetPosition(1, targetPoint);
    }
}

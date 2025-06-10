using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyFOV))]
public class EnemyFOV_Editor : Editor
{
    void OnSceneGUI()
    {
        EnemyFOV fov = (EnemyFOV)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        if (fov.visibleTarget != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.transform.position, fov.visibleTarget.position);
        }
        
    }
}

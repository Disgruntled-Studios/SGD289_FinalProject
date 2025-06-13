using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFOV : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    //public List<Transform> visibleTargets = new List<Transform>();
    public Transform visibleTarget = null;
    public Vector3 visibleTargetLastPos;
    public bool isPlayerInSight;

    void Start()
    {
        StartCoroutine("FindTargetWithDelay", .2f);
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }


    void FindVisibleTargets()
    {
        //Clear our list of targets each time this function is called
        //visibleTargets.Clear();

        if (visibleTarget != null)
        {
            if (Vector3.Distance(transform.position, visibleTarget.position) > viewRadius)
            {
                visibleTargetLastPos = visibleTarget.position;
                visibleTarget = null;
                isPlayerInSight = false;
            }
            else
            {
                return;
            }
        }

         
        //Create a list of objects that are within the viewRadius of the enemy. 
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            //set a target transform and check to see if the target is within the view angle.
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                //Final check to see if there is any obstacle obstructing the enemy's line of sight.
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask) && target.gameObject == GameManager.Instance.Player)
                {
                    //Whatever needs to happen when the target is in line of sight gets triggered here.
                    visibleTarget = target;
                    isPlayerInSight = true;
                    Debug.Log("Player is found");
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

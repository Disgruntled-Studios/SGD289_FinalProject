using UnityEngine;

public class PointValue : MonoBehaviour
{
    [SerializeField]
    public int pointValue;

    PlatformManager pm;

    void Start()
    {
        pm = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();
    }

    public void GetPoints()
    {
        pm.CalculateScore(pointValue);
    }

}

using UnityEngine;

public class AwarenessSystem : MonoBehaviour
{
    public int alertedEnemyCount;


    void Update()
    {
        alertedEnemyCount = FindObjectsByType<EnemyBehavior>(FindObjectsSortMode.None).Length;
    }
}

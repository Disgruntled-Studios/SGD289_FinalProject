using System.Collections;
using UnityEngine;

public class FPSEnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnDelay = 2f;
    [SerializeField] private float _visibleDuration = 2f;

    private bool _isActive;

    public void BeginSpawning()
    {
        _isActive = true;
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        _isActive = false;
    }

    private IEnumerator SpawnLoop()
    {
        while (_isActive)
        {
            yield return new WaitForSeconds(_spawnDelay);

            var enemy = Instantiate(_enemyPrefab, transform.position, transform.rotation);

            yield return new WaitForSeconds(_visibleDuration);

            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
    }
}

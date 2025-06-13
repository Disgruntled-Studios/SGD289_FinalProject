using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FPSEnemySpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _minSpawnDelay = 1f;
    [SerializeField] private float _maxSpawnDelay = 5f;
    [SerializeField] private float _minVisibleTime = 1f;
    [SerializeField] private float _maxVisibleTime = 5f;
    
    [Header("Spawn Variety")]
    [SerializeField] private SpawnVariety _spawnVariety;
    [SerializeField] private EnemyType _enemyType;
    
    [SerializeField] private FPSManager _manager;

    private bool _isActive;

    private void Awake()
    {
        if (!_manager)
        {
            _manager = FindAnyObjectByType<FPSManager>();
        }
    }
    
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
            var delay = Random.Range(_minSpawnDelay, _maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            var enemyObj = Instantiate(_enemyPrefab, transform.position, transform.rotation);
            var controller = enemyObj.GetComponent<FPSEnemyController>();

            var typeToSpawn = _enemyType;
            if (_spawnVariety == SpawnVariety.Random)
            {
                var values = System.Enum.GetValues(typeof(EnemyType));
                typeToSpawn = (EnemyType)values.GetValue(Random.Range(0, values.Length));
            }

            controller.Initialize(_manager, typeToSpawn);

            var visibleTime = Random.Range(_minVisibleTime, _maxVisibleTime);
            yield return new WaitForSeconds(visibleTime);

            if (enemyObj)
            {
                Destroy(enemyObj);
            }
        }
    }
}

using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FPSEnemySpawnPoint : MonoBehaviour
{
    [Header("Setup")] [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private SpawnVariety _spawnVariety;
    [SerializeField] private EnemyType _enemyType;

    private FPSEnemyController _currentEnemy;

    public void SpawnOrReset()
    {
        if (!_currentEnemy)
        {
            var obj = Instantiate(_enemyPrefab, transform.position, transform.rotation, transform);
            _currentEnemy = obj.GetComponent<FPSEnemyController>();
        }

        var assignedType = _enemyType;
        if (_spawnVariety == SpawnVariety.Random)
        {
            var types = System.Enum.GetValues(typeof(EnemyType));
            assignedType = (EnemyType)types.GetValue(Random.Range(0, types.Length));
        }

        _currentEnemy.SetType(assignedType);
        _currentEnemy.ResetForSimulation();
    }

    public bool IsCleared()
    {
        return !_currentEnemy;
    }

    public void ClearReference()
    {
        _currentEnemy = null;
    }
}

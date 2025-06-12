using UnityEngine;
using System.Collections.Generic;

public class FPSManager : MonoBehaviour
{
    public static FPSManager Instance { get; private set; }

    private float _simulationDuration = 60f;
    private int _targetScore = 100;

    private float _timeRemaining;
    private bool _isRunning;
    private int _score;

    private List<FPSEnemySpawnPoint> _spawnPoints;
    
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _spawnPoints = new List<FPSEnemySpawnPoint>(FindObjectsByType<FPSEnemySpawnPoint>(FindObjectsSortMode.None));
    }

    private void Update()
    {
        if (!_isRunning) return;

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0f)
        {
            EndSimulation();
        }
    }

    public void StartSimulation()
    {
        Debug.Log("Starting");
        _isRunning = true;
        _timeRemaining = _simulationDuration;
        _score = 0;

        foreach (var point in _spawnPoints) 
        {
            point.BeginSpawning();
        }
    }

    private void EndSimulation()
    {
        _isRunning = false;
        Debug.Log("Ending");
        
        foreach (var point in _spawnPoints) 
        {
            point.StopSpawning();
        }
    }

    public void RegisterHit()
    {
        return;
    }

    public void RegisterMishit()
    {
        return;
    }
}

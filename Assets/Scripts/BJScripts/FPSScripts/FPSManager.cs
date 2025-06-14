using UnityEngine;
using System.Collections.Generic;

public class FPSManager : MonoBehaviour
{
    public static FPSManager Instance { get; private set; }

    [SerializeField] private FPSUIController _ui;
    
    private float _simulationDuration = 100f;
    private int _targetScore = 100;

    private float _timeRemaining;
    private bool _isRunning;
    private int _score;
    private int _shotsFired;
    private int _shotsHit;
    private int _wrongHits;

    private List<FPSEnemySpawnPoint> _spawnPoints;
    private bool _firstRunComplete;
    
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
        _ui.UpdateTimer(_timeRemaining);
        
        if (_timeRemaining <= 0f || AllEnemiesCleared())
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
        _shotsFired = 0;
        _shotsHit = 0;
        _wrongHits = 0;
        _ui.UpdateScore(_score);

        foreach (var sp in _spawnPoints)
        {
            sp.SpawnOrReset();
        }
    }

    public void RegisterShotFired() => _shotsFired++;

    public void RegisterHit(int value)
    {
        _score += value;
        _shotsHit++;
        _ui.UpdateScore(_score);
    }

    public void RegisterMishit()
    {
        _wrongHits++;
        _score = Mathf.Max(0, _score - 5);
        _ui.UpdateScore(_score);
    }

    private void EndSimulation()
    {
        _isRunning = false;

        var accuracy = _shotsFired > 0 ? (float)_shotsHit / _shotsFired : 1f;

        var accuracyBonus = accuracy switch
        {
            >= 0.8f => 20,
            >= 0.7f => 10,
            _ => 0
        };

        var timeBonus = (_timeRemaining >= _simulationDuration * 0.5f) ? 20 :
            (_timeRemaining >= _simulationDuration * 0.3f) ? 10 : 0;

        var aggregateScore = _score + accuracyBonus + timeBonus;

        var passed = !_firstRunComplete && aggregateScore >= _targetScore;

        if (passed)
        {
            _firstRunComplete = true;
            _ui.ShowResult(aggregateScore, accuracy, timeBonus, accuracyBonus);
            // Recruit npc
        }
        else
        {
            _ui.ShowResult(aggregateScore, accuracy, timeBonus, accuracyBonus);
        }
    }

    private bool AllEnemiesCleared()
    {
        foreach (var sp in _spawnPoints)
        {
            if (!sp.IsCleared())
            {
                return false;
            }
        }
        
        return true;
    }
}

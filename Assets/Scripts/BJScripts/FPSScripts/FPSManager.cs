using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class FPSManager : MonoBehaviour
{
    public static FPSManager Instance { get; private set; }

    [SerializeField] private GameObject _fpsPanel;
    [SerializeField] private GameObject _startDoor;
    [SerializeField] private FPSUIController _ui;
    public FPSUIController UI => _ui;

    private const float SimulationDuration = 100f;

    private float _timeRemaining;
    private bool _isRunning;
    private int _score;
    private int _shotsFired;
    private int _shotsHit;
    private int _wrongHits;
    private int _headShotCount;

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
        _ui.UpdateTimerBar(_timeRemaining, SimulationDuration);
        
        if (_timeRemaining <= 0f || AllEnemiesCleared())
        {
            EndSimulation();
        }
    }

    public void StartSimulation()
    {
        Debug.Log("Starting");
        _fpsPanel.SetActive(true);
        _isRunning = true;
        _timeRemaining = SimulationDuration;
        _score = 0;
        _shotsFired = 0;
        _shotsHit = 0;
        _wrongHits = 0;
        _ui.UpdateEnemiesRemaining(_score);

        foreach (var sp in _spawnPoints)
        {
            sp.SpawnOrReset();
        }
    }

    public void RegisterShotFired() => _shotsFired++;

    public void RegisterHit(int value, bool isHeadshot = false)
    {
        _score += value;
        _shotsHit++;
        if (isHeadshot) _headShotCount++;
    }

    public void RegisterMishit()
    {
        _wrongHits++;
        _score = Mathf.Max(0, _score - 10);
    }

    private void EndSimulation()
    {
        _isRunning = false;

        var accuracy = _shotsFired > 0 ? (float)_shotsHit / _shotsFired : 1f;

        var accuracyBonus = accuracy switch
        {
            >= 0.95f => 30,
            >= 0.90f => 25,
            >= 0.80f => 20,
            >= 0.70f => 15,
            >= 0.60f => 10,
            >= 0.50f => 5,
            _ => 0
        };

        var percentTimeRemaining = _timeRemaining / SimulationDuration;

        var timeBonus = percentTimeRemaining switch
        {
            >= 0.75f => 30,
            >= 0.50f => 20,
            >= 0.25f => 10,
            _ => 0
        };

        var headShotBonus = _headShotCount * 3;
        
        var aggregateScore = _score + accuracyBonus + timeBonus + headShotBonus;

        var grade = CalculateGrade(aggregateScore);

        var passed = !_firstRunComplete && AllEnemiesCleared();

        if (passed)
        {
            _firstRunComplete = true;
            _ui.ShowResult(aggregateScore, accuracy, timeBonus, accuracyBonus, headShotBonus, grade, passed);
        }
        else if (!_firstRunComplete)
        {
            _ui.ShowFailResult();
        }
        else
        {
            _ui.ShowResult(aggregateScore, accuracy, timeBonus, accuracyBonus, headShotBonus, grade, passed);
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

    private string CalculateGrade(int aggregate)
    {
        return aggregate switch
        {
            >= 500 => "S",
            >= 400 => "A",
            >= 300 => "B",
            >= 200 => "C",
            >= 100 => "D",
            _ => "F"
        };
    }

    private int GetEnemiesRemaining()
    {
        var remaining = 0;
        foreach (var sp in _spawnPoints)
        {
            if (!sp.IsCleared())
            {
                remaining++;
            }
        }

        return remaining;
    }

    public void UpdateEnemyCountUI()
    {
        _ui.UpdateEnemiesRemaining(GetEnemiesRemaining());
    }
}

using UnityEngine;

public class FPSManager : MonoBehaviour
{
    public static FPSManager Instance { get; private set; }

    private float _simulationDuration = 60f;
    private int _targetScore = 100;

    private float _timeRemaining;
    private bool _isRunning;
    private int _score;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!_isRunning) return;

        _timeRemaining -= Time.deltaTime;
        Debug.Log(_timeRemaining);
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
        
        // Spawn
    }

    private void EndSimulation()
    {
        _isRunning = false;
        Debug.Log("Ending");
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

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
        if (_timeRemaining <= 0f)
        {
            EndSimulation();
        }
    }

    public void StartSimulation()
    {
        Debug.Log("Starting");
    }

    private void EndSimulation()
    {
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

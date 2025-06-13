using TMPro;
using UnityEngine;

public class FPSUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _resultText;

    private void Start()
    {
        _resultText.gameObject.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = $"{score}";
    }

    public void UpdateTimer(float timeRemaining)
    {
        _timerText.text = $"{Mathf.CeilToInt(timeRemaining)}";
    }

    public void ShowResult(bool passed)
    {
        _resultText.gameObject.SetActive(true);
        _resultText.text = passed ? "Simulation Succeeded" : "Simulation Failed";
    }
}

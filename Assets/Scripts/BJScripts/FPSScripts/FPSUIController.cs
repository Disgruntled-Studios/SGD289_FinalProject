using TMPro;
using UnityEngine;

public class FPSUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _resultText;

    [Header("Bar Controllers")]
    [SerializeField] private TimerBarController _tbc;
    [SerializeField] private StaminaBarController _sbc;

    private void Start()
    {
        _resultText.gameObject.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = $"Score: {score}";
    }

    public void UpdateTimer(float time)
    {
        _timerText.text = $"Time: {Mathf.CeilToInt(time)}";
    }

    public void ShowResult(int score, float accuracy, int timeBonus, int accBonus)
    {
        _resultText.gameObject.SetActive(true);
        _scoreText.gameObject.SetActive(false);
        _timerText.gameObject.SetActive(false);
        _resultText.text = $"SIMULATION COMPLETE\nScore: {score}\nAccuracy: {accuracy:P0}\nTime Bonus: {timeBonus}\nAccuracy Bonus: {accBonus}";
    }

    public void UpdateTimerBar(float current, float max)
    {
        _tbc.UpdateTimer(current, max);
    }

    public void UpdateStaminaBar(float current, float max)
    {
        _sbc.UpdateStamina(current, max);
    }
}

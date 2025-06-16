using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class FPSUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemyText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _resultText;

    [Header("Bar Controllers")]
    [SerializeField] private TimerBarController _tbc;
    [SerializeField] private StaminaBarController _sbc;

    private void Start()
    {
        _resultText.gameObject.SetActive(false);
        UpdateEnemiesRemaining(20);
    }

    public void UpdateEnemiesRemaining(int remaining)
    {
        _enemyText.text = $"Enemies Remaining: {remaining} / 20";
    }

    public void UpdateTimer(float time)
    {
        _timerText.text = $"Time: {Mathf.CeilToInt(time)}";
    }

    public void ShowResult(int score, float accuracy, int timeBonus, int accBonus, int headBonus, string grade, bool passed = false)
    {
        _resultText.gameObject.SetActive(true);
        _enemyText.gameObject.SetActive(false);
        _timerText.gameObject.SetActive(false);
        _resultText.text = 
            "SIMULATION COMPLETE\n" +
            $"Score: {score}\n" +
            $"Accuracy: {accuracy:P0}\n" +
            $"Time Bonus: {timeBonus}\n" +
            $"Accuracy Bonus: {accBonus}\n" +
            $"Headshot Bonus: {headBonus}\n" +
            $"Grade: {grade}";

        if (passed)
        {
            _resultText.text += "\nNPC Recruited";
        }
        else
        {
            _resultText.text += "\nFailed to Recruit NPC";
        }

        StartCoroutine(HideUI());
    }

    public void ShowFailResult()
    {
        _resultText.gameObject.SetActive(true);
        _enemyText.gameObject.SetActive(false);
        _timerText.gameObject.SetActive(false);
        _resultText.text = "SIMULATION FAILED\nYou must eliminate all enemies to pass.";

        StartCoroutine(HideUI());
    }

    public void UpdateTimerBar(float current, float max)
    {
        _tbc.UpdateTimer(current, max);
    }

    public void UpdateStaminaBar(float current, float max)
    {
        _sbc.UpdateStamina(current, max);
    }

    private IEnumerator HideUI()
    {
        yield return new WaitForSeconds(2f);
        _resultText.gameObject.SetActive(false);
        _enemyText.gameObject.SetActive(false);
        _timerText.gameObject.SetActive(false);
    }
}

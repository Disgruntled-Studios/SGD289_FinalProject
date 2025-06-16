using UnityEngine;
using UnityEngine.UI;

public class TimerBarController : MonoBehaviour
{
    [SerializeField] private Image _timerFill;

    public void UpdateTimer(float current, float max)
    {
        if (GameManager.Instance.IsInFPS)
        {
            _timerFill.fillAmount = current / max;
        }
    }
}

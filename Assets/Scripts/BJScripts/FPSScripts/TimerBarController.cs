using UnityEngine;
using UnityEngine.UI;

public class TimerBarController : MonoBehaviour
{
    [SerializeField] private Image _timerFill;

    public void UpdateTimer(float current, float max)
    {
        _timerFill.fillAmount = current / max;
    }
}

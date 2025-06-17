using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour
{
    [SerializeField] private Image _staminaFill;

    public void UpdateStamina(float current, float max)
    {
        if (GameManager.Instance.IsInFPS)
        {
            _staminaFill.fillAmount = current / max;
        }
    }
}

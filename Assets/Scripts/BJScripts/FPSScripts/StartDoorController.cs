using System.Collections;
using TMPro;
using UnityEngine;

public class StartDoorController : MonoBehaviour
{
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private Vector3 _loweredPosition;
    [SerializeField] private float _countdownTimer = 3f;
    [SerializeField] private GameObject _countdownPanel;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private GameObject _blocker;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        _countdownPanel.gameObject.SetActive(true);

        var elapsed = 0f;

        while (elapsed < _countdownTimer)
        {
            var remaining = _countdownTimer - elapsed;
            _countdownText.text = Mathf.CeilToInt(remaining).ToString();

            _doorTransform.position = Vector3.Lerp(_startPosition, _loweredPosition, elapsed / _countdownTimer);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _doorTransform.position = _loweredPosition;
        _countdownText.text = "GO!";
        _blocker.SetActive(false);
        FPSManager.Instance?.StartSimulation();
        
        yield return new WaitForSeconds(0.5f);
        _countdownPanel.gameObject.SetActive(false);
    }
}

using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class IntroUIController : MonoBehaviour, IUIPanelController
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _journalText;
    [SerializeField] private GameObject _continuePrompt;
    [TextArea] [SerializeField] private string _introText;

    [Header("Settings")]
    [SerializeField] private float _promptDelay = 2f;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _charDelay = 0.03f;
    
    private bool _hasFinishedTyping;
    
    public bool IsIntroComplete { get; private set; }
    
    public void OnPanelActivated()
    {
        ResetState();
        Time.timeScale = 0f;
        StartCoroutine(TypeText());
    }

    public void OnPanelDeactivated()
    {
        StopAllCoroutines(); // just in case
        _journalText.text = "";
        _continuePrompt.SetActive(false);
        _canvasGroup.alpha = 0f;
        Time.timeScale = 1f;
    }

    public void HandleNavigation(Vector2 input) { }

    public void HandleSubmit()
    {
        if (!_hasFinishedTyping) return;

        StartCoroutine(ClosePanel());
    }

    public void HandleCancel() { }

    public GameObject GetDefaultSelectable() => null;

    private IEnumerator TypeText()
    {
        foreach (var c in _introText)
        {
            _journalText.text += c;
            yield return new WaitForSecondsRealtime(_charDelay);
        }

        _hasFinishedTyping = true;
        yield return new WaitForSecondsRealtime(_promptDelay);
        _continuePrompt.SetActive(true);
    }

    private IEnumerator ClosePanel()
    {
        _journalText.gameObject.SetActive(false);
        _continuePrompt.SetActive(false);

        var elapsed = 0f;
        var startAlpha = _canvasGroup.alpha;

        // Fade out the intro panel over fadeDuration
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(elapsed / _fadeDuration);
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        Time.timeScale = 1f;
        IsIntroComplete = true;

        // TODO: Add the intro journal to inventory if needed
        UIManager.Instance.SetIntroComplete();
        InputManager.Instance.SwitchToDefaultInput();
        UIManager.Instance.ActivateHudPanel();
    }

    private void ResetState()
    {
        _hasFinishedTyping = false;
        IsIntroComplete = false;
        _journalText.text = "";
        _continuePrompt.SetActive(false);
        _canvasGroup.alpha = 1f;
    }

    public void OnContinuePressed()
    {
        if (_hasFinishedTyping)
        {
            HandleSubmit();
        }
    }

    public bool ShouldHandleSubmit()
    {
        return _hasFinishedTyping && gameObject.activeInHierarchy;
    }
}

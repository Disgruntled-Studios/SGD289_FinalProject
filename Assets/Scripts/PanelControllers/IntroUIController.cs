using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class IntroUIController : MonoBehaviour, IUIPanelController
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _journalText;
    [SerializeField] private GameObject _continuePrompt;
    [TextArea] [SerializeField] private string _introText;

    [Header("Settings")] 
    private const float PromptDelay = 0.5f;
    private const float FadeDuration = 2.5f;
    private const float CharDelay = 0.02f;

    [Header("Audio")] 
    [SerializeField] private AudioSource _typingAudio;
    
    private bool _hasFinishedTyping;
    
    public bool IsIntroComplete { get; private set; }

    private Coroutine _promptPulse;
    private Coroutine _typingCoroutine;
    
    public void OnPanelActivated()
    {
        ResetState();
        Time.timeScale = 0f;
        _typingCoroutine = StartCoroutine(TypeText());
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
        _typingAudio.Play();
        
        foreach (var c in _introText)
        {
            _journalText.text += c;
            yield return new WaitForSecondsRealtime(CharDelay);
        }
        
        _typingAudio.Stop();

        _hasFinishedTyping = true;
        yield return new WaitForSecondsRealtime(PromptDelay);
        _continuePrompt.SetActive(true);
        _promptPulse = StartCoroutine(AnimatePrompt());
    }

    private IEnumerator ClosePanel()
    {
        _continuePrompt.SetActive(false);

        RumbleController.Instance?.SetMotorSpeeds(0.7f, 0.9f);
        
        var elapsed = 0f;
        var startAlpha = _canvasGroup.alpha;

        // Fade out the intro panel over fadeDuration
        while (elapsed < FadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(elapsed / FadeDuration);
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);

            var rumbleLevel = Mathf.Lerp(0.7f, 0f, t);
            RumbleController.Instance?.SetMotorSpeeds(rumbleLevel, rumbleLevel);
            
            yield return null;
        }

        RumbleController.Instance?.StopAllRumbles();
        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        Time.timeScale = 1f;
        IsIntroComplete = true;

        if (_promptPulse != null) StopCoroutine(_promptPulse);
        
        GameManager.Instance.MarkIntroSeen();
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

    public void SkipTyping()
    {
        if (_hasFinishedTyping || _typingCoroutine == null) return;

        StopCoroutine(_typingCoroutine);
        _typingCoroutine = null;

        _journalText.text = _introText;
        _typingAudio.Stop();

        _hasFinishedTyping = true;
        _continuePrompt.SetActive(true);

        if (_promptPulse != null)
        {
            StopCoroutine(_promptPulse);
        }

        _promptPulse = StartCoroutine(AnimatePrompt());
    }

    public bool ShouldHandleSubmit()
    {
        return _hasFinishedTyping && gameObject.activeInHierarchy;
    }

    private IEnumerator AnimatePrompt()
    {
        var rect = _continuePrompt.transform as RectTransform;
        var baseScale = Vector3.one;
        var pulseScale = new Vector3(1.2f, 1.2f, 1.2f);
        var duration = 1.2f;

        var t = 0f;

        while (_continuePrompt.activeInHierarchy)
        {
            t += Time.unscaledDeltaTime;
            var lerp = (Mathf.Sin(t * Mathf.PI / duration) + 1f) / 2f;
            if (rect) rect.localScale = Vector3.Lerp(baseScale, pulseScale, lerp);
            yield return null;
        }

        if (rect) rect.localScale = baseScale;
    }
}

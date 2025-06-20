using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

// DOES NOT WORK

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _computer;
    [SerializeField] private CinemachineCamera _cutsceneCamera;
    [SerializeField] private float _pullSpeed = 5f;
    [SerializeField] private float _endDelay = 1.5f;
    [SerializeField] private string _nextSceneName;
    [SerializeField] private Vector3 _stretchMultiplier = new Vector3(1f, 3f, 1f);
    
    private Vector3 _originalScale;
    private float _originalDistance;
    private bool _isPulling;
    
    public void StartCutscene()
    {
        // Disable player animator (or input) as needed
        var animator = _player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.speed = 0f;
        }
    
        _originalScale = _player.localScale;
        _originalDistance = Vector3.Distance(_player.position, _computer.position);
    
        // Point player head-first once (X 90° tilt)
        _player.rotation = Quaternion.Euler(90f, _player.rotation.eulerAngles.y, _player.rotation.eulerAngles.z);
    
        // Switch camera
        if (_cutsceneCamera != null)
        {
            _cutsceneCamera.Priority = 20;
        }
    
        _isPulling = true;
    }
    
    private void Update()
    {
        if (!_isPulling)
        {
            return;
        }
    
        // Move toward computer
        var step = _pullSpeed * Time.deltaTime;
        _player.position = Vector3.MoveTowards(_player.position, _computer.position, step);
    
        // Stretch as they approach
        var currentDistance = Vector3.Distance(_player.position, _computer.position);
        var t = Mathf.InverseLerp(_originalDistance, 0f, currentDistance);
        _player.localScale = Vector3.Lerp(_originalScale, Vector3.Scale(_originalScale, _stretchMultiplier), 1f - t);
    
        if (currentDistance < 0.1f)
        {
            _isPulling = false;
            StartCoroutine(SnapBackAndDisappear());
        }
    }
    
    private IEnumerator SnapBackAndDisappear()
    {
        var snapDuration = 0.1f;
        var elapsed = 0f;
        var startScale = _player.localScale;
    
        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            var snapT = Mathf.Clamp01(elapsed / snapDuration);
            _player.localScale = Vector3.Lerp(startScale, _originalScale, snapT);
            yield return null;
        }
    
        _player.localScale = _originalScale;
        _player.position = _computer.position;
        _player.gameObject.SetActive(false);
    
        if (!string.IsNullOrEmpty(_nextSceneName))
        {
            Invoke(nameof(LoadNextScene), _endDelay);
        }
        else
        {
            Invoke(nameof(ResetCutsceneCamera), _endDelay);
        }
    }
    
    private void LoadNextScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
    
    private void ResetCutsceneCamera()
    {
        if (_cutsceneCamera != null)
        {
            _cutsceneCamera.Priority = 0;
        }
    }
}

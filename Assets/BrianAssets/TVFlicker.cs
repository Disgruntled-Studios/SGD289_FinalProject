using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TVFlicker : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material _glitchMat;
    [SerializeField] private Material _colorTestMat;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _glitchSound;
    [SerializeField] private AudioClip _colorTestSound;
    [SerializeField] private AudioSource _audioSource;
    
    [Header("Timing")]
    [SerializeField] private Vector2 _glitchDurationRange = new(7f, 10f);
    [SerializeField] private Vector2 _colorTestDurationRange = new(2f, 5f);
    
    private MeshRenderer _meshRenderer;
    private float _timer;
    private bool _isShowingColorTest;

    private const int MatSlot = 1;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        SetState(false); // Start with glitch
        StartCoroutine(StartupSequence());
    }

    private void Update()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _isShowingColorTest = !_isShowingColorTest;
                SetState(_isShowingColorTest);
                SetNextTimer();
            }
        }
    }

    private void SetState(bool showColorTest)
    {
        var mats = _meshRenderer.materials;
        if (mats.Length > MatSlot)
        {
            mats[MatSlot] = showColorTest ? _colorTestMat : _glitchMat;
            _meshRenderer.materials = mats;
        }

        if (_audioSource)
        {
            _audioSource.clip = showColorTest ? _colorTestSound : _glitchSound;
            _audioSource.Play();
        }
    }

    private void SetNextTimer()
    {
        _timer = _isShowingColorTest
            ? Random.Range(_colorTestDurationRange.x, _colorTestDurationRange.y)
            : Random.Range(_glitchDurationRange.x, _glitchDurationRange.y);
    }

    private IEnumerator StartupSequence()
    {
        SetState(false);
        yield return new WaitForSeconds(2f);

        SetState(true);
        yield return new WaitForSeconds(3f);

        _isShowingColorTest = false;
        SetState(_isShowingColorTest);
        SetNextTimer();
    }
}

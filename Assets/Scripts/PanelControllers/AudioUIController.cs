using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioUIController : MonoBehaviour, IUIPanelController
{
    [Header("Volume Sliders")] 
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    private readonly List<Slider> _sliders = new();
    private int _currentIndex;
    
    private void Awake()
    {
        _sliders.Add(_masterVolumeSlider);
        _sliders.Add(_musicVolumeSlider);
        _sliders.Add(_sfxVolumeSlider);
        
        _masterVolumeSlider.value = SoundManager.Instance.MasterVolume;
        _musicVolumeSlider.value = SoundManager.Instance.MusicVolume;
        _sfxVolumeSlider.value = SoundManager.Instance.SfxVolume;

        _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        SoundManager.Instance.SetSfxVolume(value);
    }
    
    public void OnPanelActivated()
    {
        _currentIndex = 0;
        
        for (var i = 0; i < _sliders.Count; i++)
        {
            SetSliderHighlight(_sliders[i], i == _currentIndex);
        }

        UIManager.Instance.SetEventSystemObject(_sliders[_currentIndex].gameObject);
    }

    public void OnPanelDeactivated()
    {
        foreach (var slider in _sliders)
        {
            SetSliderHighlight(slider, false);
        }
    }

    public void HandleNavigation(Vector2 input)
    {
        if (input.y > 0.5f)
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _sliders.Count - 1;
            }
        }
        else if (input.y < -0.5f)
        {
            _currentIndex++;
            if (_currentIndex >= _sliders.Count)
            {
                _currentIndex = 0;
            }
        }

        for (var i = 0; i < _sliders.Count; i++)
        {
            SetSliderHighlight(_sliders[i], i == _currentIndex);
        }

        UIManager.Instance.SetEventSystemObject(_sliders[_currentIndex].gameObject);

        var slider = _sliders[_currentIndex];
        var step = (slider.maxValue - slider.minValue) * 0.1f;

        switch (input.x)
        {
            case < -0.5f:
                slider.value -= step;
                break;
            case > 0.5f:
                slider.value += step;
                break;
        }
    }

    public void HandleSubmit()
    {
        return;
    }

    public void HandleCancel()
    {
        UIManager.Instance.ClosePauseMenu();
    }

    public GameObject GetDefaultSelectable()
    {
        return _masterVolumeSlider ? _masterVolumeSlider.gameObject : null;
    }

    private void SetSliderHighlight(Slider slider, bool highlighted)
    {
        var handle = slider.handleRect.GetComponent<Image>();
        if (handle)
        {
            handle.color = highlighted ? Color.yellow : Color.white;
        }
    }
}

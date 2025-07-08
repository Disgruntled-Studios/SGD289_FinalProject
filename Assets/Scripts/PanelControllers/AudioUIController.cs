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
    [SerializeField] private Slider _ambianceVolumeSlider;

    private readonly List<Slider> _sliders = new();
    private int _currentIndex;

    private void Awake()
    {
        _sliders.Add(_masterVolumeSlider);
        _sliders.Add(_musicVolumeSlider);
        _sliders.Add(_sfxVolumeSlider);
        _sliders.Add(_ambianceVolumeSlider);

        foreach (Slider slider in _sliders)
        {
            slider.maxValue = 1;
            slider.minValue = 0.0001f;
        }

        // _masterVolumeSlider.value = SoundManager.Instance.MasterVolume;
        // _musicVolumeSlider.value = SoundManager.Instance.MusicVolume;
        // _sfxVolumeSlider.value = SoundManager.Instance.SfxVolume;

        _masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        _musicVolumeSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        _sfxVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
        _ambianceVolumeSlider.onValueChanged.AddListener(delegate { SetAmbienceVolume(); });
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume") || PlayerPrefs.HasKey("MusicVolume") || PlayerPrefs.HasKey("SFXVolume") || PlayerPrefs.HasKey("AmbianceVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
            SetAmbienceVolume();
        }
        
    }

    private void LoadVolume()
    {
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        _ambianceVolumeSlider.value = PlayerPrefs.GetFloat("AmbianceVolume");

        SetMusicVolume();
        SetMasterVolume();
        SetSFXVolume();
        SetAmbienceVolume();
    }

    public void SetMasterVolume()
    {
        float volume = _masterVolumeSlider.value;
        SoundManager.Instance.mainMixer.SetFloat("Master", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = _musicVolumeSlider.value;
        SoundManager.Instance.mainMixer.SetFloat("Music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = _sfxVolumeSlider.value;
        SoundManager.Instance.mainMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetAmbienceVolume()
    {
        float volume = _ambianceVolumeSlider.value;
        SoundManager.Instance.mainMixer.SetFloat("Ambiance", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("AmbianceVolume", volume);
    }

    // private void OnMasterVolumeChanged(float value)
    // {
    //     SoundManager.Instance.SetMasterVolume(value);
    // }

    // private void OnMusicVolumeChanged(float value)
    // {
    //     SoundManager.Instance.SetMusicVolume(value);
    // }

    // private void OnSfxVolumeChanged(float value)
    // {
    //     SoundManager.Instance.SetSfxVolume(value);
    // }
    
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

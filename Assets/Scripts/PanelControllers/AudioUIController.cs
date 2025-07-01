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

    private readonly List<Selectable> _selectables = new();
    private int _currentIndex;
    
    private void Awake()
    {
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
        _selectables.Clear();
        _selectables.Add(_masterVolumeSlider);
        _selectables.Add(_musicVolumeSlider);
        _selectables.Add(_sfxVolumeSlider);

        _currentIndex = 0;

        UIManager.Instance.SetEventSystemObject(_selectables[_currentIndex].gameObject);
    }

    public void OnPanelDeactivated()
    {
        return;
    }

    public void HandleNavigation(Vector2 input)
    {
        if (_selectables.Count == 0) return;

        if (input.y > 0.5f)
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _selectables.Count - 1;
            }
        }
        else if (input.y < -0.5f)
        {
            _currentIndex++;
            if (_currentIndex >= _selectables.Count)
            {
                _currentIndex = 0;
            }
        }
        else
        {
            return;
        }

        UIManager.Instance.SetEventSystemObject(_selectables[_currentIndex].gameObject);
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
}

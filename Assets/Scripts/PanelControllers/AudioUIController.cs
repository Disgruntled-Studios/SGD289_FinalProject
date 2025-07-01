using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioUIController : MonoBehaviour, IUIPanelController
{
    [Header("Volume Sliders")] 
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

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
        UIManager.Instance.SetEventSystemObject(_masterVolumeSlider.gameObject);
    }

    public void OnPanelDeactivated()
    {
        throw new System.NotImplementedException();
    }

    public void HandleNavigation(Vector2 input)
    {
        throw new System.NotImplementedException();
    }

    public void HandleSubmit()
    {
        throw new System.NotImplementedException();
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

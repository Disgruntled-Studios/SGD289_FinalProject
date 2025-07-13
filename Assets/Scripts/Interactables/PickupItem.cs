using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _itemName;
    [SerializeField, TextArea] private string _additionalText;
    public string AdditionalText => _additionalText;
    [SerializeField] private Sprite _icon;

    private bool _isGun; // PlayerGun script sets this automatically
    private bool _isNote; // ReadableNote script sets this automatically

    private bool _isDevNote;

    public UnityEvent onGunPickup; // DONT USE FOR ANYTHING EXCEPT THE GUN

    [SerializeField] private GameObject _interactionPrompt;
    [SerializeField] private AudioSource _interactionAudio;

    private void Start()
    {
        _isGun = GetComponent<PlayerGun>();
        _isNote = GetComponent<ReadableNote>();

        if (TryGetComponent<ReadableNote>(out var devNote))
        {
            _isDevNote = devNote.IsDevNote;
        }
    }

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (_isGun)
        {
            player.gameObject.GetComponent<PlayerController>().GunController.HasGun = true;
            UIManager.Instance.ToggleGunImage(true);
            onGunPickup?.Invoke();
        }
        else
        {
            var item = new InventoryItem(_itemName, _isGun, _isNote, _icon, _additionalText);
            inventory.AddItem(item);
        }

        if (_isDevNote)
        {
            UIManager.Instance.StartPopUpText("Press Start To Read");
        }

        if (_interactionAudio && _interactionAudio?.clip != null)
        {
            DetachAndPlayAudio(_interactionAudio);
        }

        GameManager.Instance.PlayerController.ClearCurrentInteractable(this);

        Destroy(transform.root.gameObject);
        
        if (_interactionPrompt)
        {
            Destroy(_interactionPrompt);
        }
    }

    public void OnEnter()
    {
        if (_interactionPrompt)
        {
            _interactionPrompt.SetActive(true);
        }
    }

    public void OnExit()
    {
        if (_interactionPrompt)
        {
            _interactionPrompt.SetActive(false);
        }
    }

    private void DetachAndPlayAudio(AudioSource originalSource)
    {
        var audioObj = new GameObject("DetachedAudio")
        {
            transform =
            {
                position = originalSource.transform.position
            }
        };

        var newSource = audioObj.AddComponent<AudioSource>();
        newSource.clip = originalSource.clip;
        newSource.volume = originalSource.volume;
        newSource.pitch = originalSource.pitch;
        newSource.spatialBlend = originalSource.spatialBlend;
        newSource.outputAudioMixerGroup = originalSource.outputAudioMixerGroup;
        newSource.minDistance = originalSource.minDistance;
        newSource.maxDistance = originalSource.maxDistance;
        newSource.rolloffMode = originalSource.rolloffMode;
        newSource.rolloffMode = originalSource.rolloffMode;

        CopyAudioFilters(originalSource.gameObject, audioObj);

        newSource.PlayOneShot(_interactionAudio.clip);
        
        Destroy(audioObj, newSource.clip.length);
    }

    private void CopyAudioFilters(GameObject from, GameObject to)
    {
        // Low Pass
        if (from.TryGetComponent<AudioLowPassFilter>(out var lowPass))
        {
            var copy = to.AddComponent<AudioLowPassFilter>();
            copy.cutoffFrequency = lowPass.cutoffFrequency;
            copy.lowpassResonanceQ = lowPass.lowpassResonanceQ;
            copy.enabled = lowPass.enabled;
        }

        // High Pass
        if (from.TryGetComponent<AudioHighPassFilter>(out var highPass))
        {
            var copy = to.AddComponent<AudioHighPassFilter>();
            copy.cutoffFrequency = highPass.cutoffFrequency;
            copy.highpassResonanceQ = highPass.highpassResonanceQ;
            copy.enabled = highPass.enabled;
        }

        // Echo
        if (from.TryGetComponent<AudioEchoFilter>(out var echo))
        {
            var copy = to.AddComponent<AudioEchoFilter>();
            copy.delay = echo.delay;
            copy.decayRatio = echo.decayRatio;
            copy.wetMix = echo.wetMix;
            copy.dryMix = echo.dryMix;
            copy.enabled = echo.enabled;
        }

        // Distortion
        if (from.TryGetComponent<AudioDistortionFilter>(out var distortion))
        {
            var copy = to.AddComponent<AudioDistortionFilter>();
            copy.distortionLevel = distortion.distortionLevel;
            copy.enabled = distortion.enabled;
        }

        // Reverb
        if (from.TryGetComponent<AudioReverbFilter>(out var reverb))
        {
            var copy = to.AddComponent<AudioReverbFilter>();
            copy.reverbPreset = reverb.reverbPreset;
            copy.dryLevel = reverb.dryLevel;
            copy.room = reverb.room;
            copy.roomHF = reverb.roomHF;
            copy.roomLF = reverb.roomLF;
            copy.decayTime = reverb.decayTime;
            copy.decayHFRatio = reverb.decayHFRatio;
            copy.reflectionsLevel = reverb.reflectionsLevel;
            copy.reflectionsDelay = reverb.reflectionsDelay;
            copy.reverbLevel = reverb.reverbLevel;
            copy.reverbDelay = reverb.reverbDelay;
            copy.diffusion = reverb.diffusion;
            copy.density = reverb.density;
            copy.hfReference = reverb.hfReference;
            copy.lfReference = reverb.lfReference;
            copy.enabled = reverb.enabled;
        }

        // Chorus
        if (from.TryGetComponent<AudioChorusFilter>(out var chorus))
        {
            var copy = to.AddComponent<AudioChorusFilter>();
            copy.dryMix = chorus.dryMix;
            copy.wetMix1 = chorus.wetMix1;
            copy.wetMix2 = chorus.wetMix2;
            copy.wetMix3 = chorus.wetMix3;
            copy.delay = chorus.delay;
            copy.rate = chorus.rate;
            copy.depth = chorus.depth;
            copy.enabled = chorus.enabled;
        }
    }
}
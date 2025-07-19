using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _interactionPrompt;
    [SerializeField] private string _targetSceneName;
    [SerializeField] private string _targetCameraId;

    [SerializeField] private Material _glitchDoorMat;
    [SerializeField] private Material _glitchKnobMat;
    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private GameObject _glitchAudio;
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (string.IsNullOrEmpty(_targetSceneName)) return;

        if (_meshRenderer && _glitchDoorMat && _glitchKnobMat)
        {
            var newMats = _meshRenderer.materials;
            if (newMats.Length >= 2)
            {
                newMats[0] = _glitchDoorMat;
                newMats[1] = _glitchKnobMat;
                _meshRenderer.materials = newMats;

                // Force glitch shader to activate immediately
                newMats[0].SetFloat("_Amplitude", 1f);
                newMats[0].SetFloat("_Speed", 10f);
                newMats[0].SetFloat("_TimeOffset", Mathf.PI * 0.5f); // strong right away

                newMats[1].SetFloat("_Amplitude", 1f);
                newMats[1].SetFloat("_Speed", 10f);
                newMats[1].SetFloat("_TimeOffset", Mathf.PI * 0.5f);
            }
        }

        if (_glitchAudio)
        {
            _glitchAudio.SetActive(true);
        }
        
        GameManager.Instance.PlayerController.enabled = false;

        StartCoroutine(FadeAndLoadScene());
    }

    public void OnEnter()
    {
        _interactionPrompt.SetActive(true);
    }

    public void OnExit()
    {
        _interactionPrompt.SetActive(false);
    }

    private IEnumerator FadeAndLoadScene()
    {
        RumbleController.Instance?.TriggerPresetRumble(RumblePreset.Outro);
        
        yield return UIManager.Instance.StartCoroutine(UIManager.Instance.Fade(0f, 1f, 2.5f, () =>
        {
            RumbleController.Instance?.StopAllRumbles();
            SceneManager.LoadScene(_targetSceneName);
        }));
    }
}

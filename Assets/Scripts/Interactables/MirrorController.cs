using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class MirrorController : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _mirrorPosition;
    [SerializeField] private GameObject _mirrorPanel;

    [SerializeField] private Material[] _eyeMats;
    [SerializeField] private Material[] _hairMats;
    [SerializeField] private Material[] _shoeMats;
    [SerializeField] private Material[] _pantsMats;
    [SerializeField] private Material[] _shirtMats;

    [SerializeField] private SkinnedMeshRenderer _playerSkm;

    private const int PantsIndex = 1;
    private const int LeftShoeIndex = 2;
    private const int RightShoeIndex = 3;
    private const int EyeIndex = 4;
    private const int HairIndex = 6;
    private const int ShirtIndex = 7;

    private int _currentPantsIndex;
    private int _currentShoeIndex;
    private int _currentEyeIndex;
    private int _currentHairIndex;
    private int _currentShirtIndex;
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        CameraManager.Instance.TrySwitchToCamera("MIRRORCAM");

        var rb = player.gameObject.GetComponent<Rigidbody>();
        rb.position = _mirrorPosition.position;
        rb.rotation = _mirrorPosition.rotation;
        
        _mirrorPanel.SetActive(true);
    }

    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnClickEyes()
    {
        _currentEyeIndex = (_currentEyeIndex + 1) % _eyeMats.Length;

        var materials = _playerSkm.materials;
        materials[EyeIndex] = _eyeMats[_currentEyeIndex];
        _playerSkm.materials = materials;
    }

    public void OnClickHair()
    {
        _currentHairIndex = (_currentHairIndex + 1) % _hairMats.Length;

        var materials = _playerSkm.materials;
        materials[HairIndex] = _hairMats[_currentHairIndex];
        _playerSkm.materials = materials;
    }

    public void OnClickShoes()
    {
        _currentShoeIndex = (_currentShoeIndex + 1) % _shoeMats.Length;

        var materials = _playerSkm.materials;
        materials[LeftShoeIndex] = _shoeMats[_currentShoeIndex];
        materials[RightShoeIndex] = _shoeMats[_currentShoeIndex];
        _playerSkm.materials = materials;
    }

    public void OnClickPants()
    {
        _currentPantsIndex = (_currentPantsIndex + 1) % _pantsMats.Length;

        var materials = _playerSkm.materials;
        materials[PantsIndex] = _pantsMats[_currentPantsIndex];
        _playerSkm.materials = materials;
    }

    public void OnClickShirt()
    {
        _currentShirtIndex = (_currentShirtIndex + 1) % _shirtMats.Length;

        var materials = _playerSkm.materials;
        materials[ShirtIndex] = _shirtMats[_currentShirtIndex];
        _playerSkm.materials = materials;
    }

    public void OnClickExit()
    {
        _mirrorPanel.SetActive(false);
        CameraManager.Instance.TrySwitchToCamera("HUBMAIN");
    }

    public void OnExit()
    {
        return;
    }
}

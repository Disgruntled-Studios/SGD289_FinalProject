using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class PressureValveSystem : MonoBehaviour, IInteractable
{
    [Header("Unlocking")] 
    [SerializeField] private bool _isUnlocked = false;

    [Header("Pressure Settings")] 
    [SerializeField] private float _pressure = 0f;
    [SerializeField] private float _maxPressure = 100f;
    [SerializeField] private float _pressurePerTurn = 20f;
    [SerializeField] private float _leakRate = 10f;

    [Header("Door Movement")] 
    [SerializeField] private GameObject _door;
    [SerializeField] private Transform _doorClosedPos;
    [SerializeField] private Transform _doorOpenPos;
    [SerializeField] private float _doorOpenSpeed = 2f;

    [Header("Audio")] 
    [SerializeField] private SoundComponent _soundComponent;
    [SerializeField] private string _pressureSfx;

    [SerializeField] private Transform _highlightableObj;
    public Transform HighlightableObj => _highlightableObj;

    private bool _isBuilding = false;
    private bool _isDoorOpened = false;

    private void Update()
    {
        HandlePressureLeak();
        UpdateDoorPosition();
    }

    public void Unlock()
    {
        _isUnlocked = true;
    }
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (!_isUnlocked) return;

        if (_isDoorOpened || _isBuilding) return;

        StartCoroutine(BuildPressure());
    }

    private IEnumerator BuildPressure()
    {
        _isBuilding = true;

        _pressure += _pressurePerTurn;
        _pressure = Mathf.Min(_pressure, _maxPressure);

        _soundComponent?.PlaySFX(_pressureSfx);

        if (_pressure >= _maxPressure)
        {
            _pressure = _maxPressure;
            _isDoorOpened = true;
        }

        yield return new WaitForSeconds(0.3f);
        _isBuilding = false;
    }

    private void HandlePressureLeak()
    {
        if (_isDoorOpened || _isBuilding || _pressure <= 0f) return;

        _pressure -= _leakRate * Time.deltaTime;
        _pressure = Mathf.Max(0f, _pressure);
    }

    private void UpdateDoorPosition()
    {
        if (!_door || !_doorClosedPos || !_doorOpenPos) return;

        var t = _pressure / _maxPressure;
        var targetPos = Vector3.Lerp(_doorClosedPos.position, _doorOpenPos.position, t);
        _door.transform.position =
            Vector3.MoveTowards(_door.transform.position, targetPos, _doorOpenSpeed * Time.deltaTime);
    }

    public void OnEnter()
    {
        if (!_isUnlocked)
        {
            UIManager.Instance.StartPopUpText("It's locked by a code.", 0f);
        }
    }

    public void OnExit()
    {
        if (!_isUnlocked)
        {
            UIManager.Instance.ClearPopUpText();
        }
    }

}

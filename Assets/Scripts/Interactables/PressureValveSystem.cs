using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private float _leakRate = 5f;

    [Header("Door Movement")] 
    [SerializeField] private GameObject _door;
    [SerializeField] private float _doorClosedY = 0.5f;
    [SerializeField] private float _doorOpenY = 2f;
    [SerializeField] private float _doorOpenSpeed = 2f;

    [Header("Audio")] 
    [SerializeField] private SoundComponent _soundComponent;
    [SerializeField] private string _pressureSfx;

    [SerializeField] private Transform _highlightableObj;
    public Transform HighlightableObj => _highlightableObj;

    [SerializeField] private GameObject _spriteObject;

    private bool _isBuilding = false;
    private bool _isDoorOpened = false;

    public UnityEvent onDoorOpen;

    public void Unlock()
    {
        _isUnlocked = true;
    }

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (!_isUnlocked) return;

        if (_isDoorOpened) return;
        
        OpenDoor();
    }

    private void BuildPressure()
    {
        _pressure += _pressurePerTurn;
        _pressure = Mathf.Min(_pressure, _maxPressure);

        _soundComponent?.PlaySFX(_pressureSfx);

        if (_pressure >= _maxPressure)
        {
            OpenDoor();
        }
    }

    private void HandlePressureLeak()
    {
        if (_isDoorOpened || _isBuilding || _pressure <= 0f) return;

        _pressure -= _leakRate * Time.deltaTime;
        _pressure = Mathf.Max(0f, _pressure);
    }

    private void UpdateDoorPosition()
    {
        if (!_door) return;

        var t = _pressure / _maxPressure;
        var currentPos = _door.transform.localPosition;
        var targetY = Mathf.Lerp(_doorClosedY, _doorOpenY, t);
        var targetPos = new Vector3(currentPos.x, targetY, currentPos.z);

        _door.transform.localPosition = Vector3.MoveTowards(currentPos, targetPos, _doorOpenSpeed * Time.deltaTime);
    }

    private void OpenDoor()
    {
        Debug.Log("Opening door");
        
        _isDoorOpened = true;
        _soundComponent?.PlaySFX(_pressureSfx);

        onDoorOpen?.Invoke();
        //_pressure = _maxPressure;
    }

    private void MoveDoorToOpenPosition()
    {
        if (!_door) return;
        
        var current = _door.transform.localPosition;
        var target = new Vector3(current.x, _doorOpenY, current.z);
        _door.transform.localPosition = Vector3.MoveTowards(current, target, _doorOpenSpeed * Time.deltaTime);
    }

    public void OnEnter()
    {
        if (!_isUnlocked)
        {
            UIManager.Instance.StartPopUpText("It's locked by a code.", 0f);
        }
        else
        {
            _spriteObject.SetActive(true);
        }
    }

    public void OnExit()
    {
        if (!_isUnlocked)
        {
            UIManager.Instance.ClearPopUpText();
        }
        else
        {
            _spriteObject.SetActive(false);
        }
    }

}

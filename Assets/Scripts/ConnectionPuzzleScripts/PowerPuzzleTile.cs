using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerPuzzleTile : MonoBehaviour
{
    private bool _isPowered;
    public bool IsPowered
    {
        get => _isPowered;
        set
        {
            if (_isPowered != value)
            {
                _isPowered = value;
                ToggleConnectionMaterial(_isPowered);
                OnTileStateChanged?.Invoke();
            }
        }
    }

    [Header("Settings")]
    [SerializeField] private bool _isPowerNode;
    public bool IsPowerNode => _isPowerNode;
    [SerializeField] private bool _isConnected;
    public bool IsConnected => _isConnected;
    [SerializeField] private bool _isReceiverNode;
    public bool IsReceiverNode => _isReceiverNode;

    [Header("Materials")]
    [SerializeField] private Material _offMaterialConnector;
    [SerializeField] private Material _onMaterialConnector;
    [SerializeField] private Material _nodeOnMat;
    [SerializeField] private Material _nodeOffMat;

    [HideInInspector] 
    [SerializeField] private List<GameObject> _connectors = new();
    private MeshRenderer meshRenderer;

    public event Action OnTileStateChanged;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        for (var i = 0; i < transform.childCount; i++)
        {
            _connectors.Add(transform.GetChild(i).gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_isPowered && _isConnected) return;

        var tileRef = other.GetComponentInParent<PowerPuzzleTile>();
        if (tileRef)
        {
            _isConnected = true;
            if (tileRef.IsPowered || tileRef.IsPowerNode)
            {
                IsPowered = true;
            }
        }
    }

    public void ToggleConnectionMaterial(bool powered)
    {
        meshRenderer.material = powered ? _nodeOnMat : _nodeOffMat;
        foreach (var connector in _connectors)
        {
            var rend = connector.GetComponent<MeshRenderer>();
            if (rend)
            {
                rend.material = powered ? _onMaterialConnector : _offMaterialConnector;
            }
        }
    }
}

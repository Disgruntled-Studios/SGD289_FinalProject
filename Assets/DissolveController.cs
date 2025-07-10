using UnityEngine;

public class DissolveController : MonoBehaviour
{
    [SerializeField] private Material _dissolveMaterial;
    private const float DissolveSpeed = 1f;

    private const string DissolvePropertyName = "_DissolveAmount";

    private float _dissolveAmount;
    private bool _isDissolving;

    private void Update()
    {
        if (!_isDissolving) return;

        _dissolveAmount += Time.deltaTime * DissolveSpeed;
        _dissolveAmount = Mathf.Clamp01(_dissolveAmount);

        _dissolveMaterial.SetFloat(DissolvePropertyName, _dissolveAmount);

        if (_dissolveAmount >= 1f)
        {
            _isDissolving = false;
            Destroy(gameObject);
        }
    }

    public void StartDissolve()
    {
        var meshRenderer = GetComponent<MeshRenderer>();

        meshRenderer.material = _dissolveMaterial;
        _isDissolving = true;
        _dissolveAmount = 0f;
        _dissolveMaterial.SetFloat(DissolvePropertyName, _dissolveAmount);
    }
}

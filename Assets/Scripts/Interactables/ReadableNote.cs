using UnityEngine;
using UnityEngine.Serialization;

// Another detection script
public class ReadableNote : MonoBehaviour
{
    [SerializeField] private bool _isDevNote;
    public bool IsDevNote => _isDevNote;
}

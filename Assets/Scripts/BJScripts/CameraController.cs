using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    private readonly Vector3 _offset = new(0f, 5f, -5f);
    
    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        transform.position = _player.transform.position + _offset;
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);
    }
}

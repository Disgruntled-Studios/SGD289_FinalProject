using UnityEngine;
using UnityEngine.UI;

public class HeartBeatScroller : MonoBehaviour
{
    [SerializeField] private RawImage _image;
    [SerializeField] private float _scrollSpeed = 0.5f;

    private float _offset;

    private void Update()
    {
        _offset += _scrollSpeed * Time.deltaTime;

        if (_offset > 1f)
        {
            _offset -= 1f;
        }

        _image.uvRect = new Rect(_offset, 0, 1, 1);
    }
}

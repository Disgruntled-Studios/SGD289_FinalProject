using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridResizer : MonoBehaviour
{
    private int _columns = 3;
    private float _aspectRatio = 1f;

    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayout;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _gridLayout = GetComponent<GridLayoutGroup>();
    }

    private void Update()
    {
        ResizeCells();
    }

    private void ResizeCells()
    {
        var itemCount = _rectTransform.childCount;
        var rows = Mathf.CeilToInt((float)itemCount / _columns);

        var totalWidth = _rectTransform.rect.width;
        var totalHeight = _rectTransform.rect.height;

        var cellWidth = totalWidth / _columns;
        var cellHeight = totalHeight / rows;

        if (_aspectRatio > 0f)
        {
            cellHeight = cellWidth / _aspectRatio;
        }

        _gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        _gridLayout.spacing = Vector2.zero;
        _gridLayout.padding = new RectOffset(0, 0, 0, 0);
    }
}

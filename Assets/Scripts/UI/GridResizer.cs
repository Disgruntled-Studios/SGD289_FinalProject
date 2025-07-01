using System;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class GridResizer : MonoBehaviour
{
    private const int Columns = 4;
    private const int Rows = 5;

    private RectTransform _rectTransform;

    private void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
        Layout();
    }

    private void Update()
    {
        Layout();
    }

    private void Layout()
    {
        if (!_rectTransform)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        var childCount = transform.childCount;
        if (childCount == 0) return;

        var width = _rectTransform.rect.width;
        var height = _rectTransform.rect.height;

        var cellWidth = width / Columns;
        var cellHeight = height / Rows;

        for (var i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i) as RectTransform;
            if (!child) return;

            var row = i / Columns;
            var col = i % Columns;

            child.anchorMin = new Vector2(0, 1);
            child.anchorMax = new Vector2(0, 1);
            child.pivot = new Vector2(0, 1);

            child.sizeDelta = new Vector2(cellWidth, cellHeight);
            child.anchoredPosition = new Vector2(col * cellWidth, -row * cellHeight);
        }
    }
}

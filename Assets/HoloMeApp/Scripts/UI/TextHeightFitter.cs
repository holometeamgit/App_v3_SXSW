using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Text))]
[RequireComponent(typeof(RectTransform))]
public class TextHeightFitter : MonoBehaviour {
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _text;

    private void Update() {
        if (!_text) _text = GetComponent<Text>();
        if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();

        var desiredHeight = _text.preferredHeight;
        var size = _rectTransform.sizeDelta;
        size.y = desiredHeight;
        _rectTransform.sizeDelta = size;
    }
}
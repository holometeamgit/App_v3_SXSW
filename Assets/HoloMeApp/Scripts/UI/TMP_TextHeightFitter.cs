using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class TMP_TextHeightFitter : TMP_Text {
    [SerializeField] bool isPreferredHeight;
    [SerializeField] bool isPreferredWidth;

    private RectTransform _rectTransform;

    public string Text {
        set { text = value;
            UpdateScale();
        }

        get { return text; }
    }

    private void UpdateScale() {
        if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();

        var size = _rectTransform.sizeDelta;
        size.y = preferredHeight;
        _rectTransform.sizeDelta = size;
    }
}

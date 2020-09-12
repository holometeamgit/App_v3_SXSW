using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextStyleHandler : StyleHandler {
    TMP_Text text;

    [SerializeField] string colorTag;

    private void UpdateStyle() {
        if (!string.IsNullOrWhiteSpace(colorTag)) {
            TaggableColor taggableColor = mainDataContainer.Container.Get<TaggableColor>(colorTag);
            text.color = taggableColor?.Color ?? text.color;
        }
    }

    private void OnEnable() {
        if (text == null)
            text = GetComponent<TMP_Text>();
        styleController.OnStyleChanged += UpdateStyle;
        UpdateStyle();
    }

    private void OnDisable() {
        styleController.OnStyleChanged -= UpdateStyle;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageStyleHandler : StyleHandler {
    Image image;
    [SerializeField] string spriteTag;
    [SerializeField] string colorTag;

    private void UpdateStyle() {
        if (!string.IsNullOrWhiteSpace(colorTag)) {
            TaggableColor taggableColor = mainDataContainer.Container.Get<TaggableColor>(colorTag);
            image.color = taggableColor?.Color ?? image.color;
        }

        if (!string.IsNullOrWhiteSpace(spriteTag)) {
            TaggableSprite taggableSprite = mainDataContainer.Container.Get<TaggableSprite>(spriteTag);
            image.sprite = taggableSprite?.Sprite ?? image.sprite;
        }
    }

    private void OnEnable() {
        if (image == null)
            image = GetComponent<Image>();
        styleController.OnStyleChanged += UpdateStyle;
        UpdateStyle();
    }

    private void OnDisable() {
        styleController.OnStyleChanged -= UpdateStyle;
    }
}

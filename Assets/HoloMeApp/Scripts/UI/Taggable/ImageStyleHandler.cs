using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageStyleHandler : MonoBehaviour
{
    StyleController styleController;
    MainDataContainer mainDataContainer;
    Image image;
    [SerializeField] string spriteTag;
    [SerializeField] string colorTag;

    private void Awake() {
        image = GetComponent<Image>();

        if (styleController == null)
            styleController = FindObjectOfType<StyleController>();

        if (mainDataContainer == null)
            mainDataContainer = FindObjectOfType<MainDataContainer>();
    }

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
        styleController.OnStyleChanged.AddListener(UpdateStyle);
        UpdateStyle();
    }

    private void OnDisable() {
        styleController.OnStyleChanged.RemoveListener(UpdateStyle);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioFitterByMinSide : AspectRatioFitter
{
    private RawImage rawImage;

    public void Refresh() {
        OnRectTransformDimensionsChange();
    }

    protected override void OnRectTransformDimensionsChange() {
        if (!isActiveAndEnabled)
            return;

        var newAspectRatio = GetImgAspectRatio();
        if (newAspectRatio != 0)
            aspectRatio = newAspectRatio;

        base.OnRectTransformDimensionsChange();
    }

    private float GetImgAspectRatio() {
        if (rawImage == null)
            rawImage = GetComponent<RawImage>();
        if (rawImage == null || rawImage.texture == null)
            return default;
        return rawImage.texture.height == 0 ? 0 : rawImage.texture.width / (float)rawImage.texture.height;
    }
}

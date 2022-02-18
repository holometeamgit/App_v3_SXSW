using DynamicScrollRect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
/// <summary>
/// Cell View
/// </summary>
public class CellView : ScrollItem<ARMsgScrollItem> {

    [SerializeField]
    private CanvasGroup _loader;
    [SerializeField]
    private CanvasGroup _preview;
    [SerializeField]
    private ScreenshotView _screenshotView;

    /// <summary>
    /// Show Cell Information
    /// </summary>
    /// <param name="data"></param>
    public void Show(ARMsgJSON.Data data) {
        if (data.processing_status != ARMsgJSON.Data.COMPETED_STATUS) {
            ShowPreview(false);
        } else {
            _screenshotView.Show(data, () => ShowPreview(true), () => ShowPreview(false));
        }
    }

    private void ShowPreview(bool status) {
        _preview.alpha = status ? 1 : 0;
        _loader.alpha = status ? 0 : 1;
    }

    protected override void InitItemData(ARMsgScrollItem item) {
        Show(item.Data);
        base.InitItemData(item);
    }

}

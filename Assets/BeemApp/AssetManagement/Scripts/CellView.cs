using Beem.UI;
using DynamicScrollRect;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField]
    private GameObject _newObj;
    [SerializeField]
    private TMP_Text _processText;

    private List<IARMsgDataView> _arMsgDataViews;
    private List<IUserWebManager> _userWebManagerViews;

    /// <summary>
    /// Show Cell Information
    /// </summary>
    /// <param name="data"></param>
    public void Show(ARMsgJSON.Data data, UserWebManager userWebManager, bool isNew = false) {
        _newObj.SetActive(isNew);

        _screenshotView.Show(data, () => ShowPreview(true), (text) => { ShowPreview(false); _processText.text = text; });

        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(data));

        _userWebManagerViews = GetComponentsInChildren<IUserWebManager>().ToList();

        _userWebManagerViews.ForEach(x => x.Init(userWebManager));
    }

    private void ShowPreview(bool status) {
        _preview.alpha = status ? 1 : 0;
        _loader.alpha = status ? 0 : 1;
    }

    protected override void InitItemData(ARMsgScrollItem item) {
        Show(item.Data, item.Manager, item.IsNew);
        base.InitItemData(item);
    }

}

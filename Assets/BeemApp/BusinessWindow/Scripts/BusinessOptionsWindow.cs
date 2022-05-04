using Beem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Business View
/// </summary>
public class BusinessOptionsWindow : MonoBehaviour, IBlindView {

    [SerializeField]
    private CellView _cellView;
    [SerializeField]
    private GameObject _videoCell;

    private List<IARMsgDataView> _arMsgDataViews;
    private List<IWebRequestHandlerView> _webRequestHandlerViews;

    private UserWebManager _userWebManager;
    private BusinessProfileManager _businessProfileManager;
    private WebRequestHandler _webRequestHandler;
    private ShareLinkController _shareController = new ShareLinkController();

    private ARMsgJSON.Data _data = null;
    private bool _existPreview = true;

    private const string CTA_LINK_OPTIONS_VIEW = "CTALinkOptionsView";

    /// <summary>
    /// Show data
    /// </summary>
    /// <param name="data"></param>
    public void Show(params object[] objects) {

        if (objects != null && objects.Length > 0) {
            foreach (var item in objects) {
                if (item is bool) {
                    _existPreview = Convert.ToBoolean(item);
                } else if (item is ARMsgJSON.Data) {
                    _data = item as ARMsgJSON.Data;
                } else if (item is UserWebManager) {
                    _userWebManager = item as UserWebManager;
                } else if (item is BusinessProfileManager) {
                    _businessProfileManager = item as BusinessProfileManager;

                } else if (item is WebRequestHandler) {
                    _webRequestHandler = item as WebRequestHandler;
                }
            }
        }

        if (_data != null && _userWebManager != null && _businessProfileManager != null && _webRequestHandler != null) {

            gameObject.SetActive(true);
            _videoCell.SetActive(_existPreview);
            _cellView?.Show(_data, _userWebManager, _businessProfileManager, _webRequestHandler);
            _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();
            _arMsgDataViews.ForEach(x => x.Init(_data));
            _webRequestHandlerViews = GetComponentsInChildren<IWebRequestHandlerView>().ToList();
            _webRequestHandlerViews.ForEach(x => x.Init(_webRequestHandler));
        }
    }

    /// <summary>
    /// Open Cta Options
    /// </summary>
    public void OpenCtaOptions() {
        BlindOptionsConstructor.Show(CTA_LINK_OPTIONS_VIEW, _data, _webRequestHandler);
    }

    /// <summary>
    /// Open Cta Options
    /// </summary>
    public void OpenShareOptions() {
        if (_data.ext_content_data != null && _data.ext_content_data.Count > 0) {
            _shareController.ShareLink(_data.share_link);
        } else {
            BlindOptionsConstructor.Show(CTA_LINK_OPTIONS_VIEW, "Please add the CTA information before sharing", _data, _webRequestHandler);
        }
    }



    /// <summary>
    /// Hide
    /// </summary>

    public void Hide() {
        gameObject.SetActive(false);
    }
}

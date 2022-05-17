using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Business Btn
/// </summary>

public class ARMsgBusinessBtn : MonoBehaviour, IARMsgDataView, IUserWebManagerView, IBusinessProfileManagerView, IWebRequestHandlerView {

    private ARMsgJSON.Data _data;
    private WebRequestHandler _webRequestHandler;
    private UserWebManager _userWebManager;
    private BusinessProfileManager _businessProfileManager;

    private const string BUSINESS_OPTIONS_VIEW = "BusinessOptionsView";

    public void Init(ARMsgJSON.Data data) {
        _data = data;
    }

    public void Init(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    public void Init(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    public void Init(BusinessProfileManager businessProfileManager) {
        _businessProfileManager = businessProfileManager;
    }

    public void OnClick() {
        BlindOptionsConstructor.Show(BUSINESS_OPTIONS_VIEW, _data, _userWebManager, _businessProfileManager, _webRequestHandler, false);
    }
}

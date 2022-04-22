using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Business Btn
/// </summary>

public class ARMsgBusinessBtn : MonoBehaviour, IARMsgDataView, IUserWebManager, IWebRequestHandlerView {

    private ARMsgJSON.Data _data;
    private WebRequestHandler _webRequestHandler;
    private UserWebManager _userWebManager;

    public void Init(ARMsgJSON.Data data) {
        _data = data;
    }

    public void Init(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    public void Init(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    public void OnClick() {
        BlindOptionsConstructor.OnShow?.Invoke("BusinessOptionsView", new object[] { _data, _userWebManager, _webRequestHandler, false });
    }
}

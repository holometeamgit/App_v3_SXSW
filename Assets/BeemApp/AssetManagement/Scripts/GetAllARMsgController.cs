using System;
using UnityEngine;
using Zenject;
/// <summary>
/// Controller for get all messages
/// </summary>
public class GetAllARMsgController : IInitializable, IDisposable {
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;

    readonly SignalBus _signalBus;

    /// <summary>
    /// Constructor for GetAllARMessageController
    /// </summary>
    /// <param name="arMsgAPIScriptableObject"></param>
    /// <param name="webRequestHandler"></param>
    public GetAllARMsgController(ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler, SignalBus signalBus) {
        _arMsgAPIScriptableObject = arMsgAPIScriptableObject;
        _webRequestHandler = webRequestHandler;

        _signalBus = signalBus;
    }

    /// <summary>
    /// Get All AR Messages
    /// </summary>
    /// <param name="page"></param>
    public void GetAllArMessages(GetAllArMessagesSignal getAllArMessagesSignal) {
        _webRequestHandler.Get(GetRequestUserARMsgURL(getAllArMessagesSignal.page), OnSuccess, OnFailed);
    }

    public void Initialize() {
        _signalBus.Subscribe<GetAllArMessagesSignal>(GetAllArMessages);
    }

    public void Dispose() {
        _signalBus.Unsubscribe<GetAllArMessagesSignal>(GetAllArMessages);
    }

    private void OnSuccess(long code, string body) {
        ARMsgJSON arMsgJSON = JsonUtility.FromJson<ARMsgJSON>(body);
        if (arMsgJSON != null) {
            _signalBus.Fire(new GetAllArMessagesSuccesSignal() { arMsgJSON = arMsgJSON });
        }
    }

    private void OnFailed(long code, string body) {
        HelperFunctions.DevLogError(code + " " + body);
    }

    private string GetRequestUserARMsgURL(int page) {
        return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.UserARMessages + $"?page={page}";
    }
}

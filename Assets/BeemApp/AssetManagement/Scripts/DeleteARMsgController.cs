using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Delete ARMsgController
/// </summary>
public class DeleteARMsgController : IInitializable, IDisposable {
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;

    readonly SignalBus _signalBus;

    public DeleteARMsgController(SignalBus signalBus, ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler) {
        _arMsgAPIScriptableObject = arMsgAPIScriptableObject;
        _webRequestHandler = webRequestHandler;
        _signalBus = signalBus;
    }

    public void Initialize() {
        _signalBus.Subscribe<DeleteARMsgSignal>(DeleteARMessages);
    }

    public void Dispose() {
        _signalBus.Unsubscribe<DeleteARMsgSignal>(DeleteARMessages);
    }

    /// <summary>
    /// Delete AR Message
    /// </summary>
    /// <param name="id"></param>
    public void DeleteARMessages(DeleteARMsgSignal deleteARMsgSignal) {
        _webRequestHandler.Delete(GetRequestDeleteARMsgByIdURL(deleteARMsgSignal.idARMsg), OnSuccess , OnFailed);
    }

    private void OnSuccess(long code, string body) {
        _signalBus.Fire(new GetAllArMessagesSignal() { });
    }

    private void OnFailed(long code, string body) {
        HelperFunctions.DevLogError("Failed" + code + " " + body);
    }

    private string GetRequestDeleteARMsgByIdURL(string id) {
        return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.DeleteARMessageById.Replace("{id}", id.ToString());
    }
}
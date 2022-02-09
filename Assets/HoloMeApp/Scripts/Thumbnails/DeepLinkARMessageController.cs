﻿using System;
using Beem.ARMsg;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using UnityEngine;
using Zenject;

/// <summary>
/// Deep Link Controller for Ar-messages
/// </summary>
public class DeepLinkARMessageController : MonoBehaviour {

    [SerializeField]
    private ServerURLAPIScriptableObject _serverURLAPIScriptableObject;

    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    private WebRequestHandler _webRequestHandler;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    private void GetARMessageById(string id, Action<long, string> onSuccess, Action<long, string> onFailed) {
        HelperFunctions.DevLog("Get AR Message By Id " + id);
        _webRequestHandler.Get(GetARMessageIdUrl(id),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(code, body); },
            false);
    }

    private void ARMessageReceived(string body, Action<ARMsgJSON.Data> onReceived) {
        try {
            ARMsgJSON.Data arMsgJsonData = JsonUtility.FromJson<ARMsgJSON.Data>(body);

            HelperFunctions.DevLog("AR Message Recieved = " + body);

            onReceived?.Invoke(arMsgJsonData);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void OnOpen(string id) {
        GetARMessageById(id,
            (code, body) => Open(body),
            (code, body) => {
                DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(code.ToString(), DeepLinkRoomData.Settings.NotExist);
                DeepLinkRoomConstructor.OnShow?.Invoke(deepLinkRoomData); HelperFunctions.DevLogError(code + " " + body);
            });
    }

    private void Open(string body) {
        ARMessageReceived(body,
            (data) => {
                ARMsgPlayer.OnActivated?.Invoke(data);
            });
    }

    private void Awake() {
        StreamCallBacks.onReceiveARMsgLink += OnOpen;
    }

    private void OnDestroy() {
        StreamCallBacks.onReceiveARMsgLink -= OnOpen;
    }

    private string GetARMessageIdUrl(string id) {
        return _serverURLAPIScriptableObject.ServerURLMediaAPI + _arMsgAPIScriptableObject.ARMessageById.Replace("{id}", id.ToString());
    }
}

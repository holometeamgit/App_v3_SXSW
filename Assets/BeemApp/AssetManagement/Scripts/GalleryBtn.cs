using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Gallery Btn
/// </summary>
public class GalleryBtn : MonoBehaviour {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    [SerializeField]
    private bool open = true;

    private WebRequestHandler _webRequestHandler;

    SignalBus _signalBus;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, SignalBus signalBus) {
        _webRequestHandler = webRequestHandler;
        _signalBus = signalBus;
    }

    private void Start() {
    }

    /// <summary>
    /// On Click
    /// </summary>
    public void OnClick() {
        if (open) {
            _signalBus.Fire(new GetAllArMessagesSignal() { });
        } else {
            Hide();
        }
    }

    private void Show(GetAllArMessagesSuccesSignal signal) {
        StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
        ARMsgRecordConstructor.OnActivated?.Invoke(false);
        GalleryNotificationConstructor.OnHide?.Invoke();
        MenuConstructor.OnActivated?.Invoke(false);
        GalleryConstructor.OnShow?.Invoke(signal.arMsgJSON);
    }

    private void Hide() {
        GalleryConstructor.OnHide?.Invoke();
        MenuConstructor.OnActivated?.Invoke(true);
    }


    private void OnEnable() {
        _signalBus.Subscribe<GetAllArMessagesSuccesSignal>(Show);
    }

    private void OnDisable() {
        _signalBus.Unsubscribe<GetAllArMessagesSuccesSignal>(Show);
    }
}

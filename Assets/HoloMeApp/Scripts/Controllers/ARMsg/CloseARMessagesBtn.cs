using Beem.ARMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.UI {
    /// <summary>
    /// Close ARMessage Btn
    /// </summary>
    public class CloseARMessagesBtn : MonoBehaviour {

        [SerializeField]
        private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

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
            _signalBus.Fire<GetAllArMessagesSignal>();
        }

        private void Show(GetAllArMessagesSuccesSignal signal) {
            ARMsgARenaConstructor.OnDeactivatedARena?.Invoke();
            ARenaConstructor.onDeactivate?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            GalleryConstructor.OnShow?.Invoke(signal.arMsgJSON);
        }

        private void OnEnable() {
            _signalBus.Subscribe<GetAllArMessagesSuccesSignal>(Show);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<GetAllArMessagesSuccesSignal>(Show);
        }

    }
}
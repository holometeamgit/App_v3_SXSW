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

        private GetAllARMessageController _galleryController;

        [Inject]
        public void Construct(WebRequestHandler webRequestHandler) {
            _webRequestHandler = webRequestHandler;
        }

        private void Start() {
            _galleryController = new GetAllARMessageController(_arMsgAPIScriptableObject, _webRequestHandler);
        }

        /// <summary>
        /// On Click
        /// </summary>
        public void OnClick() {
            _galleryController.GetAllArMessages(onSuccess: Show);
        }

        private void Show(ARMsgJSON data) {
            ARMsgARenaConstructor.OnDeactivatedARena?.Invoke();
            ARenaConstructor.onDeactivate?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            GalleryConstructor.OnShow?.Invoke(data);
        }

    }
}
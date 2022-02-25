using Beem.ARMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Beem.UI {
    /// <summary>
    /// Close ARMessage Btn
    /// </summary>
    public class CloseARMessagesBtn : MonoBehaviour {

        [SerializeField]
        private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

        private WebRequestHandler _webRequestHandler;

        private WebRequestHandler GetWebRequestHandler {
            get {

                if (_webRequestHandler == null) {
                    _webRequestHandler = FindObjectOfType<WebRequestHandler>();
                }

                return _webRequestHandler;
            }
        }

        private GalleryController _galleryController;

        private void Start() {
            _galleryController = new GalleryController(_arMsgAPIScriptableObject, GetWebRequestHandler);
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

            GalleryConstructor.OnShow?.Invoke(data);
        }

    }
}
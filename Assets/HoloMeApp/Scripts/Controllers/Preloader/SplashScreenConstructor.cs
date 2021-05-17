//constructor for preloaderController in future it will be removed in DI

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem {

    public class SplashScreenConstructor : MonoBehaviour {
        [SerializeField] PnlSplashScreen _pnlSplashScreen;
        [SerializeField] GeneralAppAPIScriptableObject _generalAppAPIScriptableObject;
        [SerializeField] WebRequestHandler _webRequestHandler;

        private SplashScreenController _preloaderController;

        private void Awake() {
            VersionChecker versionChecker = new VersionChecker(_generalAppAPIScriptableObject, _webRequestHandler);
            _preloaderController = new SplashScreenController(versionChecker);

            _pnlSplashScreen.onViewEnabled += _preloaderController.Preload;
            _pnlSplashScreen.onViewStartHide += _preloaderController.OnViewStartHide;

            _preloaderController.onSentNeedUpdateApp += _pnlSplashScreen.ShowNeedUpdate;
            _preloaderController.onSignIn += _pnlSplashScreen.OnAuthorisation;
            _preloaderController.onFailSignIn += _pnlSplashScreen.OnAuthorisationErrorInvoke;
        }

        private void OnDestroy() {
            _pnlSplashScreen.onViewEnabled -= _preloaderController.Preload;
            _pnlSplashScreen.onViewStartHide -= _preloaderController.OnViewStartHide;

            _preloaderController.onSentNeedUpdateApp -= _pnlSplashScreen.ShowNeedUpdate;
            _preloaderController.onSignIn -= _pnlSplashScreen.OnAuthorisation;
            _preloaderController.onFailSignIn -= _pnlSplashScreen.OnAuthorisationErrorInvoke;
        }
    }
}

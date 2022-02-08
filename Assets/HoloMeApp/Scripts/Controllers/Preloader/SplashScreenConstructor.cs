//constructor for preloaderController in future it will be removed in DI

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem {

    public class SplashScreenConstructor : MonoBehaviour {
        [SerializeField]
        private PnlSplashScreen _pnlSplashScreen;
        [SerializeField]
        private GeneralAppAPIScriptableObject _generalAppAPIScriptableObject;

        private WebRequestHandler _webRequestHandler;

        private SplashScreenController _preloaderController;

        [Inject]
        public void Construct(WebRequestHandler webRequestHandler) {
            _webRequestHandler = webRequestHandler;
        }

        private void Awake() {
            VersionChecker versionChecker = new VersionChecker(_generalAppAPIScriptableObject, _webRequestHandler);
            _preloaderController = new SplashScreenController(versionChecker);
            _preloaderController.Preload();
            _preloaderController.onSentCanUse += SentCanUse;
            _preloaderController.onSentNeedUpdateApp += SentNeedUpdateApp;
            _preloaderController.onSignIn += OnAuthSuccess;
            _preloaderController.onFailSignIn += OnAuthFailed;
        }

        private void OnDestroy() {
            _preloaderController.onSentCanUse -= SentCanUse;
            _preloaderController.onSentNeedUpdateApp -= SentNeedUpdateApp;
            _preloaderController.onSignIn -= OnAuthSuccess;
            _preloaderController.onFailSignIn -= OnAuthFailed;
        }

        private void SentCanUse() {
            _pnlSplashScreen.Show(false);
        }

        private void SentNeedUpdateApp() {
            _pnlSplashScreen.Show(true);
        }

        private void OnAuthSuccess() {
            CreateUsernameConstructor.OnActivated?.Invoke(true);
            _pnlSplashScreen.Hide();
            _preloaderController.OnViewStartHide();
        }

        private void OnAuthFailed() {
            WelcomeConstructor.OnActivated?.Invoke(true);
            _pnlSplashScreen.Hide();
            _preloaderController.OnViewStartHide();
        }
    }
}

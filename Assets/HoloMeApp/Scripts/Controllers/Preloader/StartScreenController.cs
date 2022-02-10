using UnityEngine;
using WindowManager.Extenject;
using Zenject;

namespace Beem {
    /// <summary>
    /// StartScreen Controller
    /// </summary>
    public class StartScreenController : MonoBehaviour {
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
            SplashScreenData data = new SplashScreenData(true);
            SplashScreenConstructor.OnShow?.Invoke(data);
        }

        private void SentNeedUpdateApp() {
            SplashScreenData data = new SplashScreenData(false);
            SplashScreenConstructor.OnShow?.Invoke(data);
        }

        private void OnAuthSuccess() {
            CreateUsernameConstructor.OnShow?.Invoke();
            SplashScreenConstructor.OnHide?.Invoke();
            _preloaderController.OnViewStartHide();
        }

        private void OnAuthFailed() {
            WelcomeConstructor.OnShow?.Invoke();
            SplashScreenConstructor.OnHide?.Invoke();
            _preloaderController.OnViewStartHide();
        }
    }
}

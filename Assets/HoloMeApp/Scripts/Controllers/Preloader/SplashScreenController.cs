using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;

namespace Beem {

    public class SplashScreenController {
        public Action onSentNeedUpdateApp;
        public Action onSentCanUse;
        public Action onSignIn;
        public Action onFailSignIn;

        VersionChecker _versionChecker;

        public SplashScreenController(VersionChecker versionChecker) {
            _versionChecker = versionChecker;
        }

        public void Constructor(VersionChecker versionChecker) {
            _versionChecker = versionChecker;
        }

        public void OnViewStartHide() {
            Unsubscribe();
        }

        public void Preload() {
            Subscribe();
            _versionChecker.RequestVersion();
        }

        private void TryLogin() {
            onSentCanUse?.Invoke();
            CallBacks.onQuickLogInRequest?.Invoke();
        }

        private void OnNeedUpdateApp() {
            onSentNeedUpdateApp?.Invoke();
        }

        private void OnSignIn() {
            onSignIn?.Invoke();
        }

        private void OnSignInFail(string msg) {
            CallBacks.onLogOutRequest?.Invoke();
            onFailSignIn?.Invoke();
        }

        private void OnNeedVerification(string msg) {
            CallBacks.onLogOutRequest?.Invoke();
            OnSignInFail(msg);
        }

        private void Subscribe() {
            _versionChecker.onSentCanUse += TryLogin;
            _versionChecker.onSentNeedUpdateApp += OnNeedUpdateApp;
            CallBacks.onSignInSuccess += OnSignIn;
            CallBacks.onFail += OnSignInFail;
            CallBacks.onNeedVerification += OnNeedVerification;
        }

        private void Unsubscribe() {
            _versionChecker.onSentCanUse -= TryLogin;
            _versionChecker.onSentNeedUpdateApp -= OnNeedUpdateApp;
            CallBacks.onSignInSuccess -= OnSignIn;
            CallBacks.onFail -= OnSignInFail;
            CallBacks.onNeedVerification -= OnNeedVerification;
        }
    }
}

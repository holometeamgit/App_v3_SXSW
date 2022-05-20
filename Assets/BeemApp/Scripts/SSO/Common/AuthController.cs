using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;
using Beem.Firebase;

namespace Beem.SSO {

    /// <summary>
    /// Controller for Auth
    /// </summary>
    public class AuthController : MonoBehaviour {
        private FirebaseAuth _auth;
        private BackEndTokenController _backEndTokenController;

        private const int MAX_DIF_CREATE_USER_AND_LAST_SIGN_IN = 1000;

        public void GetFirebaseToken(Action<string> onSuccess, Action<string> onFales) {
            _backEndTokenController = _backEndTokenController ?? new BackEndTokenController();
            _backEndTokenController.GetToken(_auth.CurrentUser, onSuccess, onFales);
        }

        public void DoAfterReloadUser(Action action) {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _auth.CurrentUser.ReloadAsync().ContinueWith(task => { action?.Invoke(); }, taskScheduler);
        }

        public bool HasUser() {
            return _auth != null && _auth.CurrentUser != null;
        }

        public string GetID() {
            return _auth?.CurrentUser?.UserId;
        }

        public bool IsVerifiried() {
            HelperFunctions.DevLog(string.Format("auth {0} user {1} isVerified {2} ", _auth, _auth?.CurrentUser, _auth?.CurrentUser?.IsEmailVerified));
            return _auth != null && _auth.CurrentUser != null && _auth.CurrentUser.IsEmailVerified;
        }

        public bool IsNewUser() {
            if (_auth == null || _auth.CurrentUser == null || _auth.CurrentUser.Metadata == null)
                return false;

            HelperFunctions.DevLog("CreationTimestamp = " + _auth.CurrentUser.Metadata.CreationTimestamp);
            HelperFunctions.DevLog("LastSignInTimestamp = " + _auth.CurrentUser.Metadata.LastSignInTimestamp);
            HelperFunctions.DevLog("CreationTimestamp == LastSignInTimestamp:  " +
                (_auth.CurrentUser.Metadata.LastSignInTimestamp - _auth.CurrentUser.Metadata.CreationTimestamp < MAX_DIF_CREATE_USER_AND_LAST_SIGN_IN));

            return (_auth.CurrentUser.Metadata.LastSignInTimestamp - _auth.CurrentUser.Metadata.CreationTimestamp < MAX_DIF_CREATE_USER_AND_LAST_SIGN_IN);
        }

        public string GetEmail() {
            return _auth?.CurrentUser?.Email ?? "";
        }

        private void InitializeFirebase() {
            _auth = FirebaseAuth.DefaultInstance;
            var signInController = new SignInController(_auth);
            var signUpController = new SignUpController(_auth);
            var signInGoogleController = new SignInGoogleController(_auth);
            var signInAppleController = new SignInAppleController(_auth);
            var emailVerificationController = new EmailVerificationController(_auth);
            var phoneVerificationController = new PhoneVerificationController(_auth);
            var signInPhoneConroller = new SignInPhoneController(_auth);
            HelperFunctions.DevLog("Firebase Init Success", "Firebase");
        }

        private void SignOutCallBack() {
            _auth.SignOut();
        }

        private void OnEnable() {
            CallBacks.onSignOut += SignOutCallBack;
            FirebaseCallBacks.onInit += InitializeFirebase;
        }

        private void OnDisable() {
            CallBacks.onSignOut -= SignOutCallBack;
            FirebaseCallBacks.onInit -= InitializeFirebase;
        }
    }
}

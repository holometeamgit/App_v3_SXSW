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
        [Header("Controllers")]
        [SerializeField]
        private List<AbstractFirebaseController> abstractFirebaseControllers = new List<AbstractFirebaseController>();
        private FirebaseAuth _auth;
        private BackEndTokenController _backEndTokenController;

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

        public string GetEmail() {
            return _auth?.CurrentUser?.Email ?? "";
        }

        private void InitializeFirebase() {
            _auth = FirebaseAuth.DefaultInstance;
            foreach (AbstractFirebaseController item in abstractFirebaseControllers) {
                item.InitializeFirebase(_auth);
            }
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

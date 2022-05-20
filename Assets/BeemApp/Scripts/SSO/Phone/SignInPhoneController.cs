using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using System;

namespace Beem.SSO {

    /// <summary>
    /// SignInPhone Application
    /// </summary>
    public class SignInPhoneController : AbstractFirebaseController {

        public static string VerificationId {
            get {
                return PlayerPrefs.GetString("VerificationID");
            }
            set {
                PlayerPrefs.SetString("VerificationID", value);
            }
        }

        public SignInPhoneController(FirebaseAuth auth) : base(auth) {
        }

        private void LinkWith(string verificationCode) {
            PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(_auth);
            Credential credential = provider.GetCredential(VerificationId, verificationCode);

            if (credential == null) {
                CallBacks.onFail?.Invoke("InvalidVerificationCode");
                return;
            }

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task => {
                CheckTask(task, () => CallBacks.onFirebaseSignInSuccess?.Invoke(LogInType.Phone), CallBacks.onFail);
            }, taskScheduler);
        }

        protected override void Subscribe() {
            CallBacks.onSignInPhone += LinkWith;
        }

        protected override void Unsubscribe() {
            CallBacks.onSignInPhone -= LinkWith;
        }
    }
}

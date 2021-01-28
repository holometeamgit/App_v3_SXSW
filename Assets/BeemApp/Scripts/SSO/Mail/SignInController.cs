using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

namespace Beem.SSO {

    /// <summary>
    /// Sign In Application
    /// </summary>
    public class SignInController : AbstractFirebaseController {

        private void SignIn(string email, string password) {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            if (_auth.CurrentUser != null && _auth.CurrentUser.IsEmailVerified) {
                CallBacks.onFail?.Invoke("Email isn't verified");
            } else if (!email.Contains("@")) {
                CallBacks.onFail?.Invoke("Empty Mail");
            } else {
                _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                    CheckTask(task, CallBacks.onSignInSuccess, CallBacks.onFail);
                }, taskScheduler);
            }
        }

        protected override void Subscribe() {
            CallBacks.onSignInMail += SignIn;
        }

        protected override void Unsubscribe() {
            CallBacks.onSignInMail -= SignIn;
        }
    }
}

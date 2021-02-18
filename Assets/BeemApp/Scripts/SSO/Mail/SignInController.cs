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
            _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                CheckTask(task, () => CallBacks.onFirebaseSignInSuccess?.Invoke(LogInType.Email), CallBacks.onFail);
            }, taskScheduler);
        }

        protected override void Subscribe() {
            CallBacks.onSignInEMail += SignIn;
        }

        protected override void Unsubscribe() {
            CallBacks.onSignInEMail -= SignIn;
        }
    }
}

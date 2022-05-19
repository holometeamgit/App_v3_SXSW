using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

namespace Beem.SSO {

    /// <summary>
    /// Email Verification
    /// </summary>
    public class EmailVerificationController : AbstractFirebaseController {

        private const int SECONDS_DELAY = 60;

        public EmailVerificationController(FirebaseAuth auth) : base(auth) {
        }

        // Start is called before the first frame update
        protected override void Subscribe() {
            CallBacks.onEmailVerification += SendEmailVerification;
        }

        protected override void Unsubscribe() {
            CallBacks.onEmailVerification -= SendEmailVerification;
        }

        private void SendEmailVerification() {
            FirebaseUser user = _auth.CurrentUser;
            if (user != null) {
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                user.SendEmailVerificationAsync().ContinueWith(task => { UserTask(task); }, taskScheduler);
            }
            EmailVerificationTimer.Release(SECONDS_DELAY);
        }

        private void UserTask(Task task, Action onSuccess = null, Action<string> onFail = null) {
            LogInTaskManager firebaseTask = new LogInTaskManager();
            firebaseTask.CheckTask(task, onSuccess, onFail);
        }
    }
}

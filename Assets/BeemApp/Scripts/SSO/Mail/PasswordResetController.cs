using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Beem.SSO {

    /// <summary>
    /// Password reset
    /// </summary>
    public class PasswordResetController : AbstractFirebaseController {
        protected override void Subscribe() {
            CallBacks.onForgotAccount += ResetPassword;
        }

        protected override void Unsubscribe() {
            CallBacks.onForgotAccount -= ResetPassword;
        }

        private void ResetPassword(string email) {
            if (_auth.CurrentUser != null) {
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                _auth.SendPasswordResetEmailAsync(email).ContinueWith(task => { UserTask(task); }, taskScheduler);
            }
        }

        private void UserTask(Task task, Action onSuccess = null, Action<string> onFail = null) {
            LogInTaskManager firebaseTask = new LogInTaskManager();
            firebaseTask.CheckTask(task, onSuccess, onFail);
        }
    }
}

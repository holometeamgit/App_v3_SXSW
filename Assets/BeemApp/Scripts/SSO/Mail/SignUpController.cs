using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using System;

namespace Beem.SSO {

    /// <summary>
    /// Sign Up with email
    /// </summary>
    public class SignUpController : AbstractFirebaseController {

        private void SignUp(string profileName, string email, string password, string repeatPassword) {
            if (password != repeatPassword) {
                CallBacks.onFail?.Invoke("Passwords do not match");
            } else {
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

                _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                    CheckTask(task, () => { FirebaseSignUp(task, profileName); }, CallBacks.onFail);
                }, taskScheduler);
            }
        }

        private void FirebaseSignUp(Task<FirebaseUser> task, string profileName) {
            FirebaseUser user = task.Result;
            if (user != null) {
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                UserProfile profile = new UserProfile { DisplayName = profileName };
                user.UpdateUserProfileAsync(profile).ContinueWith(profileTask => {
                    UserTask(profileTask, () => { UpdateUserProfile(user); }, CallBacks.onFail); //TODO test can user have same name like other user?
                }, taskScheduler);
            }
        }

        private void UserTask(Task task, Action onSuccess = null, Action<string> onFail = null) {
            LogInTaskManager firebaseTask = new LogInTaskManager();
            firebaseTask.CheckTask(task, onSuccess, onFail);
        }

        private void UpdateUserProfile(FirebaseUser firebaseUser) {
            CallBacks.onSignUpSuccess?.Invoke();
            SendEmailVerification(firebaseUser);
        }

        private void SendEmailVerification(FirebaseUser user) {
            if (user != null) {
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                user.SendEmailVerificationAsync().ContinueWith(task => { UserTask(task); }, taskScheduler);
            }
        }

        protected override void Subscribe() {
            CallBacks.onSignUp += SignUp;
        }

        protected override void Unsubscribe() {
            CallBacks.onSignUp -= SignUp;
        }
    }
}

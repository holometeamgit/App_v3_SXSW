using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

namespace Beem.SSO {

    /// <summary>
    /// Firebase Phone number verification
    /// </summary>
    public class PhoneVerificationController : AbstractFirebaseController {

        private uint _phoneAuthTimeoutMs = 60000;

        public PhoneVerificationController(FirebaseAuth auth) : base(auth) {
        }

        private void VerifyPhoneNumber(string phoneNumber) {
            PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(_auth);
            provider.VerifyPhoneNumber(phoneNumber, _phoneAuthTimeoutMs, null,
              verificationCompleted: (credential) => {
                  HelperFunctions.DevLogError($"credential = {credential}");
                  HelperFunctions.DevLogError($"CurrentUser = {_auth.CurrentUser.Email}");

                  // Auto-sms-retrieval or instant validation has succeeded (Android only).
                  // There is no need to input the verification code.
                  // `credential` can be used instead of calling GetCredential().
              },
              verificationFailed: (error) => {
                  HelperFunctions.DevLogError($"error = {error}");
                  // The verification code was not sent.
                  // `error` contains a human readable explanation of the problem.
              },
              codeSent: (id, token) => {
                  SignInPhoneController.VerificationId = id;
                  HelperFunctions.DevLogError($"id = {id}, token = {token}");
                  // Verification code was successfully sent via SMS.
                  // `id` contains the verification id that will need to passed in with
                  // the code from the user when calling GetCredential().
                  // `token` can be used if the user requests the code be sent again, to
                  // tie the two requests together.
              },
              codeAutoRetrievalTimeOut: (id) => {
                  HelperFunctions.DevLogError($"id = {id}");
                  // Called when the auto-sms-retrieval has timed out, based on the given
                  // timeout parameter.
                  // `id` contains the verification id of the request that timed out.
              });

        }

        protected override void Subscribe() {
            CallBacks.onVerifiedPhone += VerifyPhoneNumber;
        }

        protected override void Unsubscribe() {
            CallBacks.onVerifiedPhone -= VerifyPhoneNumber;
        }
    }
}

using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Assertions;

namespace Beem.SSO {
    /// <summary>
    /// Sign In and Sign Up via Apple
    /// </summary>
    public class AppleSSOController : AbstractFirebaseController {

        private IAppleAuthManager _appleAuthManager;

        public AppleSSOController(FirebaseAuth auth) : base(auth) {
        }

        protected override void Subscribe() {
            CallBacks.onSignInApple += CallSignInWithApple;
        }

        protected override void Unsubscribe() {
            CallBacks.onSignInApple -= CallSignInWithApple;
        }

        void Start() {
            Init();
        }

        private void Update() {
            // Updates the AppleAuthManager instance to execute
            // pending callbacks inside Unity's execution loop
            if (this._appleAuthManager != null) {
                this._appleAuthManager.Update();
            }
        }

        private void Init() {
            if (!AppleAuthManager.IsCurrentPlatformSupported)
                return;
            var deserializer = new PayloadDeserializer();
            _appleAuthManager = new AppleAuthManager(deserializer);
        }

        private void CallSignInWithApple() {
            if (!AppleAuthManager.IsCurrentPlatformSupported)
                return;

            var rawNonce = GenerateRawNonce(32);
            var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

            var loginArgs = new AppleAuthLoginArgs(
                LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
                nonce);

            try {
                _appleAuthManager.LoginWithAppleId(loginArgs,
                    (credential) => { OnLogin(credential, rawNonce); }
                , OnError);
            } catch (Exception e) {
                HelperFunctions.DevLogError("Apple Fail " + e.Message);
                CallBacks.onFail?.Invoke("Apple Fail");
            }
        }

        private void OnLogin(ICredential credential, string rawNonce) {
            var appleIdCredential = credential as IAppleIDCredential;

            var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
            var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

            var firebaseCredential = OAuthProvider.GetCredential(
                "apple.com",
                identityToken,
                rawNonce,
                authorizationCode);

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _auth.SignInWithCredentialAsync(firebaseCredential).ContinueWith(task => {
                CheckTask(task, () => CallBacks.onFirebaseSignInSuccess?.Invoke(LogInType.Apple), CallBacks.onFail);
            }, taskScheduler);
        }


        private void OnError(IAppleError args) {
            AuthorizationErrorCode authorizationErrorCode = args.GetAuthorizationErrorCode();
            if (authorizationErrorCode == AuthorizationErrorCode.Canceled) {
                HelperFunctions.DevLogError("User canceled login");
                CallBacks.onFail?.Invoke("User canceled login");
            } else {
                HelperFunctions.DevLogError("Fail " + args.Domain);
                CallBacks.onFail?.Invoke("Fail " + args.Domain);
            }
        }

        private static string GenerateRawNonce(int count) {
            var bytes = new byte[count / 2];
            using (var rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private static string GenerateSHA256NonceFromRawNonce(string rawNonce) {
            var sha = new SHA256Managed();
            var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
            var hash = sha.ComputeHash(utf8RawNonce);

            var result = string.Empty;
            for (var i = 0; i < hash.Length; i++) {
                result += hash[i].ToString("x2");
            }

            return result;
        }
    }

}

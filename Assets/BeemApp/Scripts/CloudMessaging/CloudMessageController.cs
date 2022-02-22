using Beem.Firebase;
using Beem.Firebase.DynamicLink;
using Firebase.Messaging;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

namespace Beem.Firebase.CloudMessage {

    //INPROGRESS: This class is in progress

    /// <summary>
    /// CloudMessage Controller
    /// </summary>
    public class CloudMessageController : MonoBehaviour {

        private const string TOPIC = "Test";

        private void OnEnable() {
            FirebaseCallBacks.onInit += Subscribe;
        }

        private async void GetTokenAsync() {
            var task = FirebaseMessaging.GetTokenAsync();

            await task;

            if (task.IsCompleted) {
                HelperFunctions.DevLog("GetTokenAsync: " + task.Result);
            }
        }

        protected void Subscribe() {
            GetTokenAsync();
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
#if DEV
            FirebaseMessaging.SubscribeAsync(TOPIC);
#endif
        }

        private void OnDisable() {
            FirebaseCallBacks.onInit -= Subscribe;
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        private void OnTokenReceived(object sender, TokenReceivedEventArgs token) {
            HelperFunctions.DevLog("Received Registration Token: " + token.Token);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
            if (e.Message.Data.ContainsKey("dl")) {
                HelperFunctions.DevLog($"Message Deep Link: {e.Message.Data["dl"]}");
                DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(e.Message.Data["dl"]);
            }
        }

    }
}

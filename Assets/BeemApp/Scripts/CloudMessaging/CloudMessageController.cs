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
                GUIUtility.systemCopyBuffer = task.Result;
                HelperFunctions.DevLog("GetFCMTokenAsync: " + task.Result);
            }
        }

        protected void Subscribe() {
            GetTokenAsync();
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
            FirebaseMessaging.SubscribeAsync(TOPIC);

        }

        private void OnDisable() {
            FirebaseCallBacks.onInit -= Subscribe;
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
            FirebaseMessaging.UnsubscribeAsync(TOPIC);
        }

        private void OnTokenReceived(object sender, TokenReceivedEventArgs token) {
            GUIUtility.systemCopyBuffer = token.Token;
            HelperFunctions.DevLog("Received Registration Token: " + token.Token);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
            if (e.Message.Data.ContainsKey("dl")) {
                HelperFunctions.DevLog($"Message Deep Link: {e.Message.MessageId}");
                DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(e.Message.Data["dl"]);
            }
        }

        /*
        public async static void SendPushToTokenID(string tokenID, string title, string body) {

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var url = serverURL;
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + serverKey);


            var notification = new {
                title = title,
                body = body

            };

            var postModel = new {
                to = tokenID,
                notification = notification

            };


            var response = await client.PostAsJsonAsync(url, postModel);

            // format result json into object 
            string content = await response.Content.ReadAsStringAsync();
            string xw = (response.Content.ReadAsStringAsync().Result);

        }*/

    }
}

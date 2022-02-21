using Beem.Firebase;
using Beem.Firebase.DynamicLink;
using Firebase.Extensions;
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
                HelperFunctions.DevLogError("GetTokenAsync: " + task.Result);
            }
        }

        private async void RequestPermission() {
            var task = FirebaseMessaging.GetTokenAsync();

            await task;

            if (task.IsCompleted) {
                HelperFunctions.DevLogError("RequestPermission: " + task.Result);
            }
        }

        protected void Subscribe() {
            FirebaseMessaging.TokenRegistrationOnInitEnabled = false;
            GetTokenAsync();
            RequestPermission();
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;

            FirebaseMessaging.SubscribeAsync(TOPIC);


        }

        private void OnDisable() {
            FirebaseCallBacks.onInit -= Subscribe;
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        private void OnTokenReceived(object sender, TokenReceivedEventArgs token) {
            HelperFunctions.DevLogError("Received Registration Token: " + token.Token);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e) {

            HelperFunctions.DevLogError("Received a new message");
            var notification = e.Message.Notification;
            if (notification != null) {
                HelperFunctions.DevLogError("title: " + notification.Title);
                HelperFunctions.DevLogError("body: " + notification.Body);
                var android = notification.Android;
                if (android != null) {
                    HelperFunctions.DevLogError("android channel_id: " + android.ChannelId);
                }
            }
            if (e.Message.From.Length > 0)
                HelperFunctions.DevLogError("from: " + e.Message.From);
            if (e.Message.Link != null) {
                HelperFunctions.DevLogError("link: " + e.Message.Link.ToString());
            }
            if (e.Message.Data.Count > 0) {
                HelperFunctions.DevLogError("data:");
                foreach (KeyValuePair<string, string> iter in
                         e.Message.Data) {
                    HelperFunctions.DevLogError("  " + iter.Key + ": " + iter.Value);
                }
            }


            if (e.Message.Data.ContainsKey("dl")) {
                HelperFunctions.DevLogError($"Message Deep Link: {e.Message.Data["dl"]}");
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

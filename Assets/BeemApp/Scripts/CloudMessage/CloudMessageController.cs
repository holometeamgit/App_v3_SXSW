using Beem.Firebase;
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

        private const string SENDER_ID = "233061171188";

        private const string TOPIC = "Test";

        private void OnEnable() {
            FirebaseCallBacks.onInit += Subscribe;
        }

        protected void Subscribe() {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
            FirebaseMessaging.SubscribeAsync(TOPIC);

        }

        protected void OnDisable() {
            FirebaseCallBacks.onInit -= Subscribe;
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
            FirebaseMessaging.UnsubscribeAsync(TOPIC);
        }

        private void OnTokenReceived(object sender, TokenReceivedEventArgs token) {
            GUIUtility.systemCopyBuffer = token.Token;
            HelperFunctions.DevLogError("Received Registration Token: " + token.Token);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
            HelperFunctions.DevLogError($"Message from: {e.Message.From}");
            HelperFunctions.DevLogError($"Message ID: {e.Message.MessageId}");
            HelperFunctions.DevLogError($"Message Link: {e.Message.Link}");
            HelperFunctions.DevLogError($"Message Notification: {e.Message.Notification}");
            HelperFunctions.DevLogError($"Message MessageType: {e.Message.MessageType}");
            foreach (KeyValuePair<string, string> item in e.Message.Data) {
                HelperFunctions.DevLogError($"Message item key: {item.Key}, value: {item.Value}");
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

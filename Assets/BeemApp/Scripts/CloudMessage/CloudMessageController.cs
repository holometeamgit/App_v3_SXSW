using Beem.Firebase;
using Firebase.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Firebase.CloudMessage {
    /// <summary>
    /// CloudMessage Controller
    /// </summary>
    public class CloudMessageController : MonoBehaviour {

        private const string SENDER_ID = "233061171188";

        private const string TOPIC = "Beem";

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
        private void SendMessage() {
            FirebaseMessage message = new FirebaseMessage();
            message.To = SENDER_ID + "@fcm.googleapis.com";
            message.MessageId = "Some Id";
            message.Data.Add("my_message", "Hello World");
            message.Data.Add("my_action", "SAY HELLO");
            Firebase.Messaging.

        }*/

    }
}

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
            FirebaseMessaging.SubscribeAsync(TOPIC);
#if UNITY_ANDROID
            //StartCoroutine(LoadDLFromFCM());
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

#if UNITY_ANDROID

        private IEnumerator LoadDLFromFCM() {

            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject curActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject curIntent = curActivity.Call<AndroidJavaObject>("getIntent");

            string dl = curIntent.Call<string>("getStringExtra", "dl");
            HelperFunctions.DevLogError($"dl = {dl}");
            if (!string.IsNullOrEmpty(dl)) {
                Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
                Handheld.StartActivityIndicator();
                DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(dl);
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(1f);
            StartCoroutine(LoadDLFromFCM());
        }
#endif

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
            HelperFunctions.DevLogWarning("Received a new message");
            var notification = e.Message.Notification;
            if (notification != null) {
                HelperFunctions.DevLogWarning("title: " + notification.Title);
                HelperFunctions.DevLogWarning("body: " + notification.Body);
                var android = notification.Android;
                if (android != null) {
                    HelperFunctions.DevLogWarning("android channel_id: " + android.ChannelId);
                }
            }
            if (e.Message.From.Length > 0)
                HelperFunctions.DevLogWarning("from: " + e.Message.From);
            if (e.Message.Link != null) {
                HelperFunctions.DevLogWarning("link: " + e.Message.Link.ToString());
            }
            if (e.Message.Data.Count > 0) {
                HelperFunctions.DevLogWarning("data:");
                foreach (KeyValuePair<string, string> iter in
                         e.Message.Data) {
                    HelperFunctions.DevLogWarning("  " + iter.Key + ": " + iter.Value);
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

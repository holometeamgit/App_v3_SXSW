using Firebase.Messaging;
using UnityEngine;
using Zenject;

namespace Beem.Firebase.CloudMessage {

    /// <summary>
    /// CloudMessage Controller
    /// </summary>
    public class FCMController : MonoBehaviour {
        [SerializeField]
        private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

        private WebRequestHandler _webRequestHandler;


        private const string TOPIC = "Test";

        private const string MSG_TYPE = "message_type_beem";
        private const string GALLERY = "gallery";

        [Inject]
        public void Construct(WebRequestHandler webRequestHandler) {
            _webRequestHandler = webRequestHandler;
        }

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
            if (e.Message.Data.ContainsKey(MSG_TYPE) && e.Message.Data[MSG_TYPE] == GALLERY) {
                GalleryNotificationController galleryNotificationController = new GalleryNotificationController(_arMsgAPIScriptableObject, _webRequestHandler);
                galleryNotificationController.SetData(e.Message.Data);
            }
        }
    }
}

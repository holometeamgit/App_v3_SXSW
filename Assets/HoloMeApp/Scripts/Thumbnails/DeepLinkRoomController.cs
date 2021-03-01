using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using UnityEngine;

public class DeepLinkRoomController : MonoBehaviour {
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] ServerURLAPIScriptableObject serverURLAPIScriptableObject;
    [SerializeField] VideoUploader videoUploader;
    [SerializeField] AccountManager accountManager;
    [SerializeField] DeepLinkHandler deepLinkHandler;

    private void GetMyRoom() {
        webRequestHandler.GetRequest(GetMyRoomIdUrl(),
            (code, body) => MyRoomIdRecieved(body),
            (code, body) => HelperFunctions.DevLogError(code + " " + body),
            accountManager.GetAccessToken().access);
    }

    private void MyRoomIdRecieved(string body) {
        try {
            RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(body);
            //room?roomid=string
            string uri = serverURLAPIScriptableObject.FirebaseDynamicLinkAPI + "/" + roomJsonData.id;
            CreateLink(serverURLAPIScriptableObject.FirebaseDynamicLinkAPI, uri, StreamCallBacks.onMyRoomLinkReceived);
        } catch { }
    }

    private void CreateLink(string prefix, string baseLink, Action<string> onMyRoomLinkReceived) {
        HelperFunctions.DevLog("baseLink = " + baseLink);
        HelperFunctions.DevLog("prefix = " + prefix);
        var components = new DynamicLinkComponents(
      // The base Link.
      new Uri(baseLink),
      // The dynamic link URI prefix.
      prefix) {
            IOSParameters = new IOSParameters(Application.identifier),
            AndroidParameters = new AndroidParameters(Application.identifier),
        };

        var options = new DynamicLinkOptions {
            PathLength = DynamicLinkPathLength.Short
        };

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        DynamicLinks.GetShortLinkAsync(components, options).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("GetShortLinkAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("GetShortLinkAsync encountered an error: " + task.Exception);
                return;
            }

            // Short Link has been created.
            ShortDynamicLink link = task.Result;
            Debug.LogFormat("Generated short link: {0}", link.Url);
            onMyRoomLinkReceived?.Invoke(link.Url.AbsoluteUri);
        }, taskScheduler);
    }

    private void Awake() {
        StreamCallBacks.onGetMyRoomLink += GetMyRoom;
    }

    private string GetMyRoomIdUrl() {
        return serverURLAPIScriptableObject.ServerURLMediaAPI + videoUploader.GetRoom;
    }
}

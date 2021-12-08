using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Crosstales.BWF.Model;
using Crosstales.BWF;
using UnityEngine.Events;

public class PnlStreamChat : AgoraMessageReceiver {
    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    AgoraRTMChatController agoraRTMChatController;

    [SerializeField]
    GameObject chatMessagePrefabRef;

    [SerializeField]
    RectTransform Content;

    [SerializeField]
    ScrollRect scrollRect;

    [SerializeField]
    VerticalLayoutGroup verticalLayoutGroup;

    [SerializeField]
    private UnityEvent OnMessageAdded;

    Stack<GameObject> chatMessagePool = new Stack<GameObject>();

    private void Awake() {
        agoraRTMChatController.AddMessageReceiver(this);
    }

    private void OnDestroy() {
        agoraRTMChatController.RemoveMessageReceiver(this);
    }

    public void OnEnable() {
        StartRefreshLayoutRoutine();
    }

    public void SendChatMessage(string message) {
        if (string.IsNullOrWhiteSpace(message)) {
            return;
        }

        bool rudeWordDetected = BWFManager.Contains(message, ManagerMask.Domain | ManagerMask.BadWord);
        string censoredText = BWFManager.ReplaceAll(message, ManagerMask.Domain | ManagerMask.BadWord);

        if (rudeWordDetected)
            HelperFunctions.DevLog("Rude word detected new string = " + censoredText);

        ChatMessageJsonData chatMessageJsonData;

        if (!agoraController.IsLive && agoraController.IsChannelCreator) {
            chatMessageJsonData = new ChatMessageJsonData { userName = "", message = "Channel must be live to post comments" };
        } else {
            chatMessageJsonData = new ChatMessageJsonData { userName = agoraRTMChatController.UserName, message = censoredText };
        }
        CreateChatMessageGO(chatMessageJsonData);

        if (agoraController.IsLive)
            agoraRTMChatController.SendMessageToChannel(JsonUtility.ToJson(chatMessageJsonData));

        StartRefreshLayoutRoutine();
    }

    public override void ReceivedChatMessage(string data) {
        AgoraStreamMessageCommonType agoraStreamMessage = JsonParser.CreateFromJSON<AgoraStreamMessageCommonType>(data);
        if (agoraStreamMessage.requestID == AgoraMessageRequestIDs.IDChatMessage) {
            var chatMessageJsonData = JsonParser.CreateFromJSON<ChatMessageJsonData>(data);
            CreateChatMessageGO(chatMessageJsonData);
            StartRefreshLayoutRoutine();
        }
    }

    private void CreateChatMessageGO(ChatMessageJsonData chatMessageJsonData) {
        var newMessageGO = GetChatMessage();
        newMessageGO.GetComponent<ChatMessageView>().CreateMessage(chatMessageJsonData.userName, chatMessageJsonData.message);
        OnMessageAdded?.Invoke();
    }

    private GameObject GetChatMessage() {
        if (chatMessagePool.Count == 0) {
            return Instantiate(chatMessagePrefabRef, Content, false);
        } else {
            var returnObject = chatMessagePool.Pop();
            returnObject.GetComponent<RectTransform>().SetAsLastSibling();
            returnObject.SetActive(true);
            return returnObject;
        }
    }

    private void ReturnChatMessageToPool(GameObject message) {
        if (!chatMessagePool.Contains(message))//This is neccessary for now to add messages back if the user doesn't go online but posts in the chat it stops duplicate entries
        {
            chatMessagePool.Push(message);
            message.gameObject.SetActive(false);
            message.GetComponent<ChatMessageView>().DestroyMessage();
        }
    }

    public override void OnDisconnected() {
        for (int i = 0; i < Content.childCount; i++) {
            ReturnChatMessageToPool(Content.GetChild(i).gameObject);
        }

        GetComponent<AnimatedTransition>()?.DoMenuTransition(false);
    }

    void StartRefreshLayoutRoutine() {
        if (gameObject.activeSelf) {
            StopAllCoroutines();
            StartCoroutine(RefreshLayoutGroup());
        }
    }

    IEnumerator RefreshLayoutGroup() {
        verticalLayoutGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        verticalLayoutGroup.enabled = true;
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
    }
}

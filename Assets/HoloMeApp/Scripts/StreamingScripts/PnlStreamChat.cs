using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Crosstales.BWF.Model;
using Crosstales.BWF;

public class PnlStreamChat : AgoraMessageReceiver
{
    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    AgoraRTMChatController agoraRTMChatController;

    [SerializeField]
    GameObject chatMessagePrefabRef;

    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    RectTransform Content;

    [SerializeField]
    ScrollRect scrollRect;

    [SerializeField]
    VerticalLayoutGroup verticalLayoutGroup;

    Stack<GameObject> chatMessagePool = new Stack<GameObject>();

    //[SerializeField]
    //UnityEvent OnMessageRecieved; //For displaying notifications

    private void Awake()
    {
        agoraRTMChatController.AddMessageReceiver(this);
    }

    private void OnDestroy()
    {
        agoraRTMChatController.RemoveMessageReceiver(this);
    }

    public void OnEnable()
    {
        StartRefreshLayoutRoutine();
    }

    public void SendChatMessage(string message)
    {
        bool rudeWordDetected = BWFManager.Contains(message, ManagerMask.Domain | ManagerMask.BadWord);
        string censoredText = BWFManager.ReplaceAll(message, ManagerMask.Domain | ManagerMask.BadWord);

        if (rudeWordDetected)
            HelperFunctions.DevLog("Rude word detected new string = " + censoredText);

        ChatMessageJsonData chatMessageJsonData = new ChatMessageJsonData { userName = agoraRTMChatController.UserName, message = censoredText };
        CreateChatMessageGO(chatMessageJsonData);
        agoraRTMChatController.SendMessageToChannel(JsonUtility.ToJson(chatMessageJsonData));

        inputField.text = "";
        StartRefreshLayoutRoutine();
    }

    public override void ReceivedChatMessage(string data)
    {
        AgoraStreamMessageCommonType agoraStreamMessage = JsonParser.CreateFromJSON<AgoraStreamMessageCommonType>(data);
        if (agoraStreamMessage.requestID == AgoraMessageRequestIDs.IDChatMessage)
        {
            var chatMessageJsonData = JsonParser.CreateFromJSON<ChatMessageJsonData>(data);
            CreateChatMessageGO(chatMessageJsonData);
            StartRefreshLayoutRoutine();
        }
    }

    private void CreateChatMessageGO(ChatMessageJsonData chatMessageJsonData)
    {
        var newMessageGO = GetChatMessage();
        //chatMessages.Add(newMessageGO);
        newMessageGO.transform.Find("txtUserName").GetComponent<TextMeshProUGUI>().text = chatMessageJsonData.userName;
        newMessageGO.transform.Find("txtMessage").GetComponent<TextMeshProUGUI>().text = chatMessageJsonData.message;
    }

    private GameObject GetChatMessage()
    {
        if (chatMessagePool.Count == 0)
        {
            return Instantiate(chatMessagePrefabRef, Content, false);
        }
        else
        {
            var returnObject = chatMessagePool.Pop();
            returnObject.SetActive(true);
            return returnObject;
        }
    }

    private void ReturnChatMessageToPool(GameObject message)
    {
        chatMessagePool.Push(message);
        message.gameObject.SetActive(false);
    }

    public override void OnDisconnected()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            ReturnChatMessageToPool(Content.GetChild(i).gameObject);
        }

        GetComponent<AnimatedTransition>()?.DoMenuTransition(false);
    }

    void StartRefreshLayoutRoutine()
    {
        if (gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(RefreshLayoutGroup());
        }
    }

    IEnumerator RefreshLayoutGroup()
    {
        verticalLayoutGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        verticalLayoutGroup.enabled = true;
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
    }
}

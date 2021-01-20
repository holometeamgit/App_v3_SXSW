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
        //inputField.ActivateInputField();
    }

    public void SendChatMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            inputField.text = string.Empty;
            return;
        }

        bool rudeWordDetected = BWFManager.Contains(message, ManagerMask.Domain | ManagerMask.BadWord);
        string censoredText = BWFManager.ReplaceAll(message, ManagerMask.Domain | ManagerMask.BadWord);

        if (rudeWordDetected)
            HelperFunctions.DevLog("Rude word detected new string = " + censoredText);

        if (!agoraController.IsLive && agoraController.IsChannelCreator)
            censoredText = "Channel must be live to post comments";

        ChatMessageJsonData chatMessageJsonData = new ChatMessageJsonData { userName = agoraRTMChatController.UserName, message = censoredText };
        CreateChatMessageGO(chatMessageJsonData);

        if (agoraController.IsLive)
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
        newMessageGO.transform.Find("txtUserName").GetComponent<TextMeshProUGUI>().text = chatMessageJsonData.userName;
        newMessageGO.transform.Find("txtMessage").GetComponent<TextMeshProUGUI>().text = chatMessageJsonData.message;
    }

    private GameObject GetChatMessage()
    {
        print("CHAT MESSAGE COUNT = " + chatMessagePool.Count);
        if (chatMessagePool.Count == 0)
        {
            print("CREATING A NEW CHAT MESSAGE");
            return Instantiate(chatMessagePrefabRef, Content, false);
        }
        else
        {
            print("POPPING MESSAGE" + chatMessagePool.Count);
            var returnObject = chatMessagePool.Pop();
            print("MESSAGE POPPED" + chatMessagePool.Count);
            returnObject.GetComponent<RectTransform>().SetAsLastSibling();
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

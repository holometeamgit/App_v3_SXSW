using agora_rtm;
using io.agora.rtm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgoraRTMChatController : MonoBehaviour
{
    [SerializeField]
    private UserWebManager userWebManager;

    private string appId;
    public string UserName { get; private set; }

    private RtmClient rtmClient;
    private RtmChannel channel;
    private RtmClientEventHandler clientEventHandler;
    private RtmChannelEventHandler channelEventHandler;

    bool loggedIn;

    void Initialise()
    {
        clientEventHandler = new RtmClientEventHandler();
        channelEventHandler = new RtmChannelEventHandler();

        rtmClient = new RtmClient(appId, clientEventHandler);
#if UNITY_EDITOR
        rtmClient.SetLogFile("./rtm_log.txt");
#endif

        //rtmClient = RtmWrapper.Instance;
        clientEventHandler.OnLoginSuccess += OnClientLoginSuccessHandler;
        channelEventHandler.OnJoinSuccess += OnJoinSuccessHandler;
        channelEventHandler.OnMessageReceived += OnChannelMessageReceivedHandler;
        //clientEventHandler.OnQueryStatusReceived += Rtm_OnQueryStatusReceived;
        //clientEventHandler.OnChannelMemberCountReceived += Rtm_OnChannelMemberCountReceived;
        //clientEventHandler.OnMemberChanged += Rtm_OnMemberChanged;

        //if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(appId))
        //{
        //    Debug.LogError("We need a username and appId to login");
        //    yield break;
        //}
    }

    public void Init(string appId)
    {
        this.appId = appId;
        Initialise();
    }

    public void Login(string token)
    {
        if (loggedIn)
            return;

        UserName = userWebManager.GetUsername();
        rtmClient.Login(token, UserName);
    }

    public void Logout()
    {
        //SendMessageToChat(userName + " logged out of the rtm", Message.MessageType.info);
        if (loggedIn)
            rtmClient?.Logout();
    }

    public void JoinChannel(string channelName)
    {
        //Debug.Log("User: " + userName + " joined room: " + channelName);

        //if (!loggedIn)
        //    Debug.LogError("You need to login before you can join a channel");

        StartCoroutine(JoinChannelWhenLoggedIn(channelName));

    }

    IEnumerator JoinChannelWhenLoggedIn(string channelName)
    {
        while (!loggedIn) //Wait till logged in
        {
            yield return null;
        }

        channel = rtmClient.CreateChannel(channelName, channelEventHandler);
        channel.Join();
    }

    public void LeaveChannel()
    {
        if (loggedIn && rtmClient != null && channel != null)
            channel.Leave();
        OnStreamDisconnected();
    }

    void OnChannelMessageReceivedHandler(int id, string userName, TextMessage message)
    {
        //SendMessageToChat(userName + ": " + msg, Message.MessageType.playerMessage);

        //TODO: Use the userName param to specify the user name as currently it's compiles in msg string via PnlStreamChat SendChatMessage 

        HelperFunctions.DevLog("Message received RTM");

        foreach (AgoraMessageReceiver agoraMessageReceiver in messageReceivers)
        {
            agoraMessageReceiver.ReceivedChatMessage(message.GetText());
        }
    }

    void OnJoinSuccessHandler(int id)
    {

    }

    public void OnClientLoginSuccessHandler(int id)
    {
        HelperFunctions.DevLog("RTM Logged into chat");
        loggedIn = true;
        //SendMessageToChat(userName + " logged into the rtm", Message.MessageType.info);
    }

    #region Messaging system

    public void SendMessageToChannel(string message)
    {
        channel.SendMessage(rtmClient.CreateMessage(message));
        //rtm.SendChannelMessage(channelName, text);
        HelperFunctions.DevLog("Message Sent");
    }

    public void OnStreamMessageError(uint userId, int streamId, int code, int missed, int cached)
    {
        HelperFunctions.DevLog($"Stream message error! Code = {code}");
    }

    List<AgoraMessageReceiver> messageReceivers = new List<AgoraMessageReceiver>();
    public void AddMessageReceiver(AgoraMessageReceiver agoraMessageReceiver)
    {
        if (!messageReceivers.Contains(agoraMessageReceiver))
        {
            HelperFunctions.DevLog("Message received");
            messageReceivers.Add(agoraMessageReceiver);
        }
        else
        {
            Debug.LogError("Tried to add the same messageReceiver");
        }
    }

    public void RemoveMessageReceiver(AgoraMessageReceiver agoraMessageReceiver)
    {
        if (messageReceivers.Contains(agoraMessageReceiver))
        {
            messageReceivers.Remove(agoraMessageReceiver);
        }
        else
        {
            Debug.LogError("Tried to remove messageReceiver but wasn't in collection");
        }
    }

    //public void OnStreamMessageRecieved(uint userId, int streamId, string data, int length)
    //{
    //    HelperFunctions.DevLog($"Message recieved {data}");

    //    foreach (AgoraMessageReceiver agoraMessageReceiver in messageReceivers)
    //    {
    //        agoraMessageReceiver.ReceivedChatMessage(data);
    //    }
    //}

    public void OnStreamDisconnected()
    {
        foreach (AgoraMessageReceiver agoraMessageReceiver in messageReceivers)
        {
            agoraMessageReceiver.OnDisconnected();
        }
    }

    #endregion

    private void OnDestroy()
    {
        Logout();
    }

    void OnApplicationQuit()
    {
        if (channel != null)
        {
            channel.Dispose();
            channel = null;
        }
        if (rtmClient != null)
        {
            rtmClient.Dispose();
            rtmClient = null;
        }
    }
}

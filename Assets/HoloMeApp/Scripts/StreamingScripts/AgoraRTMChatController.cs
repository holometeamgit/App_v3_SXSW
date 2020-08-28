using io.agora.rtm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgoraRTMChatController : MonoBehaviour
{
    [SerializeField]
    private string token = "";

    public string userName = "temp1";

    private IRtmWrapper rtm;
    private IRtmChannel channel;

    IEnumerator Initialize(string appId)
    {
        while (RtmWrapper.Instance == null)
            yield return null;

        rtm = RtmWrapper.Instance;
        rtm.OnLoginSuccess += Rtm_OnLoginSuccess;
        rtm.OnJoinSuccess += Rtm_OnJoinSuccess;
        rtm.OnMessageReceived += Rtm_OnMessageReceived;
        rtm.OnQueryStatusReceived += Rtm_OnQueryStatusReceived;
        rtm.OnChannelMemberCountReceived += Rtm_OnChannelMemberCountReceived;
        rtm.OnMemberChanged += Rtm_OnMemberChanged;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(appId))
        {
            Debug.LogError("We need a username and appId to login");
            yield break;
        }

        Debug.Log("Before calling rtm");
        rtm.Login(appId, token, userName);
    }

    public void Login(string appId)
    {
        StartCoroutine(Initialize(appId));
    }

    public void Logout()
    {
        //SendMessageToChat(userName + " logged out of the rtm", Message.MessageType.info);
        rtm.Logout();
    }

    public void JoinChannel(string channelName)
    {
        //Debug.Log("User: " + userName + " joined room: " + channelName);

        if (!rtm.LoggedIn)
            Debug.LogError("You need to login before you can join a channel");

        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                channel = rtm.JoinChannel(channelName);
            });
        }
    }

    public void LeaveChannel()
    {
        rtm.LeaveChannel(channel);
        OnStreamDisconnected();
    }


    private void Rtm_OnMemberChanged(string userName, string channelId, bool joined)
    {
        //SendMessageToChat(userName + (joined ? " joined" : " left") + " channel " + channelId, Message.MessageType.info);
    }

    private void Rtm_OnChannelMemberCountReceived(long requestId, List<ChannelMemberCount> channelMembers)
    {
        foreach (var ch in channelMembers)
        {
            //SendMessageToChat("Channel: " + ch.channelId + " has: " + ch.count + " member(s)", Message.MessageType.info);
        }
    }

    private void Rtm_OnQueryStatusReceived(long requestId, PeerOnlineStatus peersStatus, int peerCount, int errorCode)
    {
        //SendMessageToChat("Query users: " + queryUsersBox.text + ": " + (peersStatus.onlineState == 0), Message.MessageType.info);
    }

    private void Rtm_OnMessageReceived(string userName, string msg)
    {
        //SendMessageToChat(userName + ": " + msg, Message.MessageType.playerMessage);

        //TODO: Use the userName param to specify the user name as currently it's compiles in msg string via PnlStreamChat SendChatMessage 

        foreach (AgoraMessageReceiver agoraMessageReceiver in messageReceivers)
        {
            agoraMessageReceiver.ReceivedChatMessage(msg);
        }
    }

    public void Rtm_OnJoinSuccess()
    {
        //SendMessageToChat(userName + " joined the " + channelName + " channel", Message.MessageType.info);
    }

    public void Rtm_OnLoginSuccess()
    {
        //SendMessageToChat(userName + " logged into the rtm", Message.MessageType.info);
    }

    #region Messaging system

    public void SendMessageToChannel(string message)
    {
        channel.SendMessage(message);
        //rtm.SendChannelMessage(channelName, text);
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
            Debug.LogError("Tried to remove messageReceiver but wasn't in colection");
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
}

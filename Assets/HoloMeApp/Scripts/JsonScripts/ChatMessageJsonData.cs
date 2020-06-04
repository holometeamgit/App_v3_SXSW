using System;

[Serializable]
public class ChatMessageJsonData : AgoraStreamMessage
{
    public ChatMessageJsonData()
    {
        requestID = AgoraMessageRequestIDs.IDChatMessage;
    }

    public string userName;
    public string message;

}

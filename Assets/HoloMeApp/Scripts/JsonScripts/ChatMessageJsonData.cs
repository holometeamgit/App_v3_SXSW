using System;

[Serializable]
public class ChatMessageJsonData : AgoraStreamMessageCommonType
{
    public ChatMessageJsonData()
    {
        requestID = AgoraMessageRequestIDs.IDChatMessage;
    }

    public string userName;
    public string message;

}

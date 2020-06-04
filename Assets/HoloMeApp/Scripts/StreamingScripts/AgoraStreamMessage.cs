using System;

[Serializable]
public abstract class AgoraStreamMessage
{
    public int requestID = AgoraMessageRequestIDs.IDUnassigned;

    //public virtual void Package(string data)
    //{
    //    data += RequestID;
    //}

    //public void Parse(string data)
    //{
    //    var agoraStreamMessage = JsonParser.CreateFromJSON<AgoraStreamMessage>(data);
    //    if (agoraStreamMessage != null)
    //    {
    //        if (agoraStreamMessage.requestID == requestID)
    //        {
    //            ParseDataType();
    //        }
    //    }
    //}
}

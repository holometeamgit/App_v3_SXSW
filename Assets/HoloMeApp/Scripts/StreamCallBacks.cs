using System;

/// <summary>
/// Call backs for streams
/// </summary>
public class StreamCallBacks {
    #region deep link
    // from app to controller
    public static Action<string> onReceivedDeepLink = delegate { };
    #endregion

    //when start stream on Agora
    public static Action<StreamStartResponseJsonData> onLiveStreamCreated = delegate { };
    public static Action onLiveStreamFinished = delegate { };

    public static Action<DeepLinkHandler.Params> onSelectedMode = delegate { };

    public static Action<int> onOpenComment = delegate { };

    public static Action onCloseComments = delegate { };
    public static Action onCommentsClosed = delegate { };
}

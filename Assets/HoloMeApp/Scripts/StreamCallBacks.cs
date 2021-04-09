using System;


/// <summary>
/// Call backs for streams
/// </summary>
public class StreamCallBacks {
    #region deep link
    public static Action<string> onRoomLinkReceived = delegate { };
    public static Action onOpenRoom = delegate { };

    public static Action<string> onStreamLinkReceived = delegate { };
    public static Action onOpenStream = delegate { }; // can subscribe on stream pnl

    public static Action onCancelOpenContent = delegate { };
    #endregion

    public static Action onGetMyRoomLink = delegate { };
}

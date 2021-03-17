using System;


/// <summary>
/// Call backs for streams
/// </summary>
public class StreamCallBacks {
    public static Action<string> onRoomLinkReceived = delegate { };
    public static Action onOpenRoom = delegate { };
    public static Action onCancelOpenRoom = delegate { };
    public static Action onGetMyRoomLink = delegate { };
    public static Action<string> onMyRoomLinkReceived = delegate { };
}

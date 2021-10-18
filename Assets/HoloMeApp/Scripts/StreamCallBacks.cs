﻿using System;


/// <summary>
/// Call backs for streams
/// </summary>
public class StreamCallBacks {
    #region deep link room
    // from app to controller
    public static Action<string> onUsernameLinkReceived = delegate { };
    public static Action<RoomJsonData> onRoomDataReceived = delegate { };
    // from app to controller: notification that broadcaster stop room 
    public static Action onRoomClosed = delegate { };

    //from controller
    public static Action<string> onShowPopUpRoomOnline = delegate { };
    public static Action<int> onUpdateUserCount = delegate { };
    public static Action<string> onShowPopUpRoomOffline = delegate { };
    public static Action<string> onShowPopUpRoomEnded = delegate { };
    public static Action onClosePopUp = delegate { };

    //from ui
    public static Action onOpenRoom = delegate { };
    public static Action onShareRoom = delegate { };
    public static Action onPopUpStartClosing = delegate { };
    public static Action onPopUpClosed = delegate { };
    public static Action onPopUpStartOpen = delegate { };
    #endregion

    #region deep link stream
    public static Action<string> onStreamLinkReceived = delegate { };
    public static Action onOpenStream = delegate { }; // can subscribe on stream pnl

    public static Action onCancelOpenContent = delegate { };
    #endregion

    #region
    public static Action<string, string, string, bool> onPlayLiveStream = delegate { };
    #endregion

    public static Action onGetLastRoomLink = delegate { };
    public static Action<string> onGetRoomLink = delegate { };
    public static Action<string, string> onGetStreamLink = delegate { };
    public static Action<StreamJsonData.Data> onGetPrerecordedLink = delegate { };

    //when start stream on Agora
    public static Action<StreamStartResponseJsonData> onRoomCreated = delegate { };
    public static Action<StreamStartResponseJsonData> onLiveStreamCreated = delegate { };
    public static Action onLiveStreamFinished = delegate { };

    public static Action<int> onOpenComment = delegate { };

    public static Action onCloseComments = delegate { };
    public static Action onCommentsClosed = delegate { };
}

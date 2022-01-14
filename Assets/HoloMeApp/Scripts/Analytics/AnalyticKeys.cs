public class AnalyticKeys {
    public const string KeyRegistrationComplete = "Registration_Complete";
    public const string KeyRegistrationDropOff = "Registration_Dropped_Off";
    public const string KeyUserSignup = "User_Signup";
    public const string KeyUserLogin = "User_Login";

    public const string KeySignUpTapped = "Sign_Up_Tapped";
    public const string KeyProfileCreated = "Profile_Created";
    public const string KeyEmailOptIn = "Email_Opt_In";
    //public const string KeyNotificationOptIn = "Notification_Opt_In"; //TODO future opt in notifications panel
    //public const string KeyNotificationMaybeLater = "Notification_Maybe_Later"; //TODO future opt in notifications panel

    public const string KeyShareHologramMediaPressed = "Share_Hologram_Recording_Pressed";
    public const string KeySnapshotShared = "Hologram_Snapshot_Shared";
    public const string KeyVideoShared = "Hologram_Video_Shared";

    public const string KeyHologramViewPercentage = "Hologram_View_Percentage";
    public const string KeyHologramLiveViewTime = "Hologram_Live_View_Time";
    public const string KeyViewerCountUpdate = "Viewer_Count_Update";
    public const string KeyMaxViewerCount = "Max_Viewer_Count";

    public const string KeyBeemMeSelected = "Beem_Me_Pressed";
    public const string KeyBeemMeUploadStarted = "Upload_Beem_Me";
    public const string KeyBeemMeConversionComplete = "Conversion_Complete";
    public const string KeyBeemMePlaced = "Place_Beem_Me";
    public const string KeyBeemMeShared = "Share_Beem_Me";

    public const string KeyGoLive = "Go_Live"; //When a broadcaster clicks 'go live' from the main menu
    public const string KeyLiveStarted = "Live_Started"; //When a broadcaster clicks 'go live' from the main menu and starts to record a video

    public const string KeyStartPerformance = "Start_Performance"; //The user clicks 'watch now', before placing the hologram in their environment
    public const string KeyPerformanceLoaded = "Performance_Loaded";
    public const string KeyPerformanceNotEnded = "Performance_Not_Ended"; //Whole performance wasn't watched
    public const string KeyPerformanceEnded = "Performance_Ended"; //Watched full hologram

    public const string KeyPurchasePressed = "Purchase_Pressed";
    public const string KeyPurchaseSuccessful = "Purchase_Successful";
    public const string KeyPurchaseFailed = "Purchase_Failed";
    public const string KeyPurchaseCancelled = "Purchase_Cancelled";
    public const string KeyShareEventPressed = "Share_Event_Pressed";

    public const string KeySessionLength = "Session_Length";
    public const string KeyViewLengthOfStream = "Length_Of_Stream_Viewed";

    public const string KeyHomeScreen = "Home_Screen";
    public const string KeySettingsPanel = "Settings";
}

public class AnalyticParameters {
    public const string ParamUserID = "User_ID";
    public const string ParamProductID = "Product_ID";
    public const string ParamBroadcasterUserID = "Broadcaster_User_ID";
    public const string ParamPerformanceID = "Performance_ID";
    public const string ParamUserEmail = "User_Email";
    public const string ParamUserType = "User_Type";
    public const string ParamIsRoom = "Is_Room";
    public const string ParamViewer = "Viewer";
    public const string ParamBroadcaster = "Broadcaster";
    public const string ParamVideoName = "Video_Name";
    public const string ParamEventName = "Event_Name";
    public const string ParamProductPrice = "Amount";
    public const string ParamTime = "Time";
    public const string ParamDate = "Date";
    public const string ParamSignUpMethod = "Sign_Up_Method";
    public const string ParamViewerCount = "Viewer_Count";
    public const string ParamChannelName = "Channel_Name";
    //public const string ParamNotificationsOptIn = "notificationOptIn"; //TODO future opt in notifications panel
}
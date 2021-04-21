public class AnalyticKeys {
    public const string KeyRegistrationComplete = "registrationComplete";
    public const string KeyRegistrationDropOff = "registrationDroppedOff";
    public const string KeyUserSignup = "userSignup";
    public const string KeyUserLogin = "userLogin";

    public const string KeyShareHologramMediaPressed = "shareHologramRecordingPressed";
    public const string KeySnapshotShared = "hologramSnapshotShared";
    public const string KeyVideoShared = "hologramVideoShared";
    
    public const string KeyHologramViewPercentage = "hologramViewPercentage";
    public const string KeyHologramLiveViewTime = "hologramLiveViewTime";
    
    public const string KeyGoLive = "goLive"; //When a broadcaster clicks 'go live' from the main menu
    public const string KeyLiveStarted = "liveStarted"; //When a broadcaster clicks 'go live' from the main menu and starts to record a video
    
    public const string KeyStartPerformance = "startPerformance"; //The user clicks 'watch now', before placing the hologram in their environment
    public const string KeyPerformanceLoaded = "performanceLoaded";
    public const string KeyPerformanceNotEnded = "performanceNotEnded";
    public const string KeyPerformanceEnded = "performanceEnded"; //Watched full hologram
    
    public const string KeyPurchasePressed = "purchasePressed";
    public const string KeyPurchaseSuccessful = "purchaseSuccessful";
    public const string KeyPurchaseFailed = "purchaseFailed";
    public const string KeyPurchaseCancelled = "purchaseCancelled";
    public const string KeyShareEventPressed = "shareEventPressed";
    
    public const string KeySessionLength = "sessionLength";
}

public class AnalyticParameters{
    public const string ParamUserID = "userID";
    public const string ParamProductID = "productId";
    public const string ParamBroadcasterUserID = "broadcasterUserId";
    public const string ParamPerformanceID = "performanceID";
    public const string ParamUserEmail = "userEmail";
    public const string ParamUserType = "userType";
    public const string ParamViewer = "viewer";
    public const string ParamBroadcaster = "broadcaster";
    public const string ParamVideoName = "videoName";
    public const string ParamEventName = "eventName";
    public const string ParamProductPrice = "amount";
    public const string ParamTime = "time";
    public const string ParamDate = "date";

    public const string ParamHomeScreen = "homeScreen";
    public const string ParamSettingsPanel = "settings";
}
using System;
using UnityEngine.Events;

namespace Beem.ARMsg {

    /// <summary>
    /// Static callbacks for ARMsg
    /// </summary>
    public class CallBacks {
        public static Action OnScreenshotRequest = delegate { };
        public static Action OnStartFirstStepRecord = delegate { };
        public static Action OnStartRecord = delegate { };
        public static Action OnStopRecord = delegate { };
        public static Action OnVideoReadyPlay = delegate { };
        public static Action OnReplay = delegate { };

        public static Func<string> OnGetVideoRecordedFilePath;
        public static Func<string> OnGetBGImgFilePath;
        public static Func<string> OnGetLastARMsgShareLink;
        public static Func<ARMsgJSON.Data> OnGetARMsgByID;
        public static Func<ARMsgJSON.Data> OnGetLastReadyARMsgData;
        public static Action OnCancelLastGetARMsgById;

        public static Action OnARMsgUpdloaded = delegate { };
        public static Action<float> OnARMsgUpdloadedProcessing = delegate { };
        public static Action OnARMsgUploadedError = delegate { };
        public static Action OnAllARMsgСanceled = delegate { };

        #region UI
        public static Action OnUpdloadingUIOpened = delegate { };
        public static Action OnARMsgProcessingCheck = delegate { };
        public static Action OnDeleteLastARMsgActions = delegate { };
        public static Action OnCancelAllARMsgActions = delegate { };
        #endregion

        #region ARmsg Webrequest
        public static Action<ARMsgJSON> OnARMsgListReceived = delegate { };
        public static Action<ARMsgJSON.Data> OnARMsgByIdReceived = delegate { };
        public static Action<string> OnARMsgByIdDeleted = delegate { };
        #endregion

        #region Camera Settings Action
        public static Action onSwitchCameraClicked = delegate { };

        public static Func<bool> onCanSwitchCamera;
        public static Action<int> onCameraSwitched;
        public static Func<int> onGetCurrentCameraID;
        #endregion

        #region Record Timer Settings action
        public static Action onSwitchRecordTimerClicked = delegate { };
        public static Action onGetCurrevRecordTimerClicked = delegate { };

        public static Action<int> onRecordTimerSet;
        #endregion
    }
}
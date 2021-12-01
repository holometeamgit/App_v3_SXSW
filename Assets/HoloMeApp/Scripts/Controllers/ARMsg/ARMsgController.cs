using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace Beem.ARMsg {
    public class ARMsgController {
        private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;
        private WebRequestHandler _webRequestHandler;

        private ActionWrapper _cancelUploadARMsg;
        private ActionWrapper _cancelGetUserARMsgURL;
        private ActionWrapper _cancelGetARMsgById;
        private ActionWrapper _cancelDeleteARMsgById;

        private ARMsgJSON.Data _lastUploadedARMsgJSON;
        private ARMsgJSON.Data _lastLoadedARMsgJSON;
        private const string LAST_UPLOADED_ARMSG_NAME = "lastUploadedARMsg";

        public ARMsgController(ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler) {
            Contructor(arMsgAPIScriptableObject, webRequestHandler);
        }

        public void UploadARMsg() {
            UploadARMsg(CallBacks.OnGetBGImgFilePath?.Invoke(), CallBacks.OnGetVideoRecordedFilePath?.Invoke());
        }

        public void GetUserARMsgs() {
            RequestGetUserARMsgs();
        }

        public void GetARMsgById(string id) {
            RequestGetARMsgById(id);
        }

        public void GetLastUploadedARMsgInfo() {
            if (_lastUploadedARMsgJSON == null)
                return;

            GetARMsgById(_lastUploadedARMsgJSON.id);
        }

        public bool CheckContainLastUploadedARMsg() {
            return _lastUploadedARMsgJSON != null;
        }

        public void OnCancelAll() {
            _cancelUploadARMsg?.InvokeAction();
            _cancelGetUserARMsgURL?.InvokeAction();
            _cancelGetARMsgById?.InvokeAction();
            _cancelDeleteARMsgById?.InvokeAction();

            _lastUploadedARMsgJSON = null;
            PlayerPrefs.DeleteKey(LAST_UPLOADED_ARMSG_NAME);
            PlayerPrefs.Save();

            CallBacks.OnAllARMsgСanceled?.Invoke();
        }

        public void DeleteLastARMsg() {
            if (_lastUploadedARMsgJSON != null)
                DeleteARMsgById(_lastUploadedARMsgJSON.id);
        }

        public void DeleteARMsgById(string id) {
            ReqestDeleteARMsgById(id);
        }

        public string GetReadyShareLink() {
            return _lastLoadedARMsgJSON?.ar_message_s3_link ?? null;
        }

        public ARMsgJSON.Data GetLastReadyARMsgData() {
            return _lastLoadedARMsgJSON;
        }

        //[Inject]
        private void Contructor(ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler) {
            _arMsgAPIScriptableObject = arMsgAPIScriptableObject;
            _webRequestHandler = webRequestHandler;
            if (PlayerPrefs.HasKey(LAST_UPLOADED_ARMSG_NAME)) {
                try {
                    _lastUploadedARMsgJSON = JsonUtility.FromJson<ARMsgJSON.Data>(PlayerPrefs.GetString(LAST_UPLOADED_ARMSG_NAME));
                } catch { }
            }
        }

        #region UploadARMsg

        private void UploadARMsg(string pathToImgBG, string pathToVideoFile) {

            HelperFunctions.DevLog("Try upload: " + pathToImgBG + " " + " video: " + pathToVideoFile);

            _cancelUploadARMsg?.InvokeAction();
            _cancelUploadARMsg = new ActionWrapper();


            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add(_arMsgAPIScriptableObject.ImgBGFieldName, pathToImgBG);
            content.Add(_arMsgAPIScriptableObject.SourceVideoFieldName, pathToVideoFile);

            _webRequestHandler.PostMultipart(GetPostRequestUploadARMsgURL(), content, PostUploadARMsgCallback, ErrorPostUploadARMsgCallback, onCancel: _cancelUploadARMsg);
        }

        private void PostUploadARMsgCallback(long code, string body) {
            HelperFunctions.DevLog("PostUploadARMsgCallback" + body);
            try {
                _lastUploadedARMsgJSON = JsonUtility.FromJson<ARMsgJSON.Data>(body);
                PlayerPrefs.SetString(LAST_UPLOADED_ARMSG_NAME, body);
                PlayerPrefs.Save();
                HelperFunctions.DevLog("id: " + _lastUploadedARMsgJSON.id);
            } catch { }

            CallBacks.OnARMsgUpdloaded?.Invoke();
        }

        private void ErrorPostUploadARMsgCallback(long code, string body) {
            HelperFunctions.DevLogError(string.Format("Can't upload ARMsg. {0} {1}", code, body));
        }

        private string GetPostRequestUploadARMsgURL() {
            //https://stackoverflow.com/questions/56155603/how-to-upload-multiple-files-to-a-server-using-unitywebrequest-post
            return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.ARMessageUpload;
        }
        #endregion

        #region GetUserARMsgs
        private void RequestGetUserARMsgs() {
            _cancelGetUserARMsgURL?.InvokeAction();
            _cancelGetUserARMsgURL = new ActionWrapper();
            _webRequestHandler.Get(GetRequestUserARMsgURL(), GetUserARMsgsCallback, ErrorGetUserARMsgsCallback, onCancel: _cancelGetUserARMsgURL);
        }

        private void GetUserARMsgsCallback(long code, string body) {
            HelperFunctions.DevLog(body);
            ARMsgJSON arMsgJSON = JsonUtility.FromJson<ARMsgJSON>(body);
            if (arMsgJSON != null) {
                HelperFunctions.DevLog(string.Format("ARMsgs list with {0} elements", arMsgJSON.results.Count));
                CallBacks.OnARMsgListReceived?.Invoke(arMsgJSON);
            }
        }

        private void ErrorGetUserARMsgsCallback(long code, string body) {
            HelperFunctions.DevLogError(string.Format("Can't get ARMsg list. {0} {1}", code, body));
        }

        private string GetRequestUserARMsgURL() {
            return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.UserARMessages;
        }
        #endregion

        #region GetARMsgById
        private void RequestGetARMsgById(string id) {
            _cancelGetARMsgById?.InvokeAction();
            _cancelGetARMsgById = new ActionWrapper();

            _webRequestHandler.Get(GetRequestARMsgByIdURL(id), GetARMsgByIdCallback,
                (code, body) => { string currectId = id; ErrorGetARMsgByIdCallback(currectId, code, body); },
                onCancel: _cancelGetARMsgById);
        }

        private void GetARMsgByIdCallback(long code, string body) {
            HelperFunctions.DevLog(body);
            _lastLoadedARMsgJSON = JsonUtility.FromJson<ARMsgJSON.Data>(body);
            if (_lastLoadedARMsgJSON != null) {
                HelperFunctions.DevLog(string.Format("ARMsg by id {0} was received", _lastLoadedARMsgJSON.id));
                CallBacks.OnARMsgByIdReceived?.Invoke(_lastLoadedARMsgJSON);
            }
        }

        private void ErrorGetARMsgByIdCallback(string id, long code, string body) {
            OnCancelAll();
            HelperFunctions.DevLogError(string.Format("Can't get ARMsg by id = {0}. {1} {2}", id, code, body));
        }

        private string GetRequestARMsgByIdURL(string id) {
            return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.ARMessageById.Replace("{id}", id.ToString());
        }
        #endregion

        #region DeleteARMsgById
        private void ReqestDeleteARMsgById(string id) {
            _cancelDeleteARMsgById?.InvokeAction();
            _cancelDeleteARMsgById = new ActionWrapper();

            _webRequestHandler.Delete(GetRequestDeleteARMsgByIdURL(id),
                (code, body) => { string currectId = id; DeleteRMsgByIdCallback(currectId, code, body); },
                (code, body) => { string currectId = id; ErrorDeleteRMsgByIdCallback(currectId, code, body); },
                onCancel: _cancelDeleteARMsgById);
        }

        private void DeleteRMsgByIdCallback(string id, long code, string body) {
            HelperFunctions.DevLog(string.Format("ARMsg by id {0} was deleted", id));
            CallBacks.OnARMsgByIdDeleted?.Invoke(id);
        }

        private void ErrorDeleteRMsgByIdCallback(string id, long code, string body) {
            HelperFunctions.DevLogError(string.Format("Can't delete ARMsg by id = {0}. {1} {2}", id, code, body));
        }

        private string GetRequestDeleteARMsgByIdURL(string id) {
            return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.DeleteARMessageById.Replace("{id}", id.ToString());
        }
        #endregion

        ~ARMsgController() {
            OnCancelAll();
        }
    }
}
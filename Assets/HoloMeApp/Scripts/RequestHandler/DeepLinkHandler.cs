using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;
using Zenject;

/// <summary>
/// Handler for all deeplinks
/// </summary>
public class DeepLinkHandler : MonoBehaviour {

    [SerializeField]
    private VideoUploader _videoUploader;

    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    public enum Params {
        room,
        message,
        live,
        stadium,
        prerecorded,
        username
    }

    public Action<string> VerificationDeepLinkActivated;
    public Action<string, string> PasswordResetConfirmDeepLinkActivated;
    public Action<ServerAccessToken> OnCompleteSSOLoginGetted;

    private GetRoomController _getRoomController;
    private GetStadiumController _getStadiumController;
    private GetPrerecordedController _getPrerecordedController;
    private GetARMsgController _getARMsgController;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _getRoomController = new GetRoomController(_videoUploader, webRequestHandler);
        _getStadiumController = new GetStadiumController(_videoUploader, webRequestHandler);
        _getPrerecordedController = new GetPrerecordedController(_videoUploader, webRequestHandler);
        _getARMsgController = new GetARMsgController(_arMsgAPIScriptableObject, webRequestHandler);
    }

    private void DeepLinkActivated(string uriStr) {

        Uri uri = new Uri(uriStr);

        HelperFunctions.DevLog("Deep link: " + uriStr);
        GetContentsParameters(uri);
    }

    private void Awake() {
        StreamCallBacks.onReceivedDeepLink += DeepLinkActivated;
        Application.deepLinkActivated += DeepLinkActivated;
    }

    private void Start() {

        HelperFunctions.DevLog("Application.absoluteURL: " + Application.absoluteURL);

        if (!string.IsNullOrEmpty(Application.absoluteURL)) {
            // Cold start and Application.absoluteURL not null so process Deep Link.
            DeepLinkActivated(Application.absoluteURL);
        }
    }

    private void OnDestroy() {
        StreamCallBacks.onReceivedDeepLink -= DeepLinkActivated;
        Application.deepLinkActivated -= DeepLinkActivated;
    }

    private void GetContentsParameters(Uri uri) {
        if (ContainParam(uri, Params.message.ToString())) {
            string messageId = GetParam(uri, Params.message.ToString());
            _getARMsgController.GetARMsgById(messageId, DeepLinkVideoConstructor.OnShow, DeepLinkVideoConstructor.OnShowError);
        } else if (ContainParam(uri, Params.room.ToString())) {
            string username = GetParam(uri, Params.room.ToString());
            _getRoomController.GetRoomByUsername(username, DeepLinkStreamConstructor.OnShow, DeepLinkStreamConstructor.OnShowError);
        } else if (ContainParam(uri, Params.username.ToString())) {
            string username = GetParam(uri, Params.username.ToString());
            _getRoomController.GetRoomByUsername(username, DeepLinkStreamConstructor.OnShow, DeepLinkStreamConstructor.OnShowError);
        } else if (ContainParam(uri, Params.live.ToString())) {
            string username = GetParam(uri, Params.live.ToString());
            _getStadiumController.GetStadiumByUsername(username, DeepLinkStreamConstructor.OnShow, DeepLinkStreamConstructor.OnShowError);
        } else if (ContainParam(uri, Params.stadium.ToString())) {
            string username = GetParam(uri, Params.stadium.ToString());
            _getStadiumController.GetStadiumByUsername(username, DeepLinkStreamConstructor.OnShow, DeepLinkStreamConstructor.OnShowError);
        } else if (ContainParam(uri, Params.prerecorded.ToString())) {
            string slug = GetParam(uri, Params.prerecorded.ToString());
            _getPrerecordedController.GetPrerecordedBySlug(slug, DeepLinkVideoConstructor.OnShow, DeepLinkVideoConstructor.OnShowError);
        }
        StreamCallBacks.onSelectedMode?.Invoke(Params.room);
    }

    private bool ContainParam(Uri uri, string parameter) {
        return ContainFolder(uri, parameter) || ContainQueryParam(uri, parameter);
    }

    private string GetParam(Uri uri, string parameter) {
        if (ContainQueryParam(uri, parameter)) {
            return GetQueryParam(uri, parameter);
        } else if (ContainFolder(uri, parameter)) {
            return GetFolderId(uri, parameter);
        }
        return null;
    }

    private bool ContainFolder(Uri uri, string parameter) {
        return uri.LocalPath.Contains(parameter);
    }

    private string GetFolderId(Uri uri, string parameter) {
        string localPath = uri.LocalPath;
        localPath = localPath.Substring(1, localPath.Length - 1);
        string[] split = localPath.Split('/');
        for (int i = 0; i < split.Length; i++) {
            if (split[i].Contains(parameter) && i < split.Length - 1) {
                return split[split.Length - 1];
            }
        }
        return string.Empty;
    }

    private bool ContainQueryParam(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter) != null;
    }

    private string GetQueryParam(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter);
    }
}
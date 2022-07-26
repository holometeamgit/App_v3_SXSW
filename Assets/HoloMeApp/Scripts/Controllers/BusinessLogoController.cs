using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using System.Threading.Tasks;
using System.Threading;
using Zenject;

/// <summary>
/// Controller helps upload and download logo for business account
/// </summary>
public class BusinessLogoController {
    private Sprite _logo;
    private Sprite _selectedLogoFromDevice;
    private BusinessProfileManager _businessProfileManager;
    private AuthorizationAPIScriptableObject _authorizationAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;

    public BusinessLogoController(BusinessProfileManager businessProfileManager, AuthorizationAPIScriptableObject authorizationAPIScriptableObject, WebRequestHandler webRequestHandler) {
        _businessProfileManager = businessProfileManager;
        _authorizationAPIScriptableObject = authorizationAPIScriptableObject;
        _webRequestHandler = webRequestHandler;
        CallBacks.onSelectLogoFromDevice += SelectNewImg;
        CallBacks.onUploadSelectedLogo += OnUploadSelectedLogo;
        CallBacks.onRemoveLogo += OnRemove;
        CallBacks.onLoadLogo += LoadLogo;
        CallBacks.onSignInSuccess += LoadLogo;

        CallBacks.hasLogoOnDevice = HasLogo;
        CallBacks.getLogoOnDevice = GetCurrentLogoImage;
        CallBacks.getSelectedLogoOnDevice = GetSelectedLogo;
    }

    private Sprite GetCurrentLogoImage() {
        return _logo;
    }

    private Sprite GetSelectedLogo() {
        return _selectedLogoFromDevice;
    }

    private bool HasLogo() {
        return _logo != null;
    }

    #region select img
    private void SelectNewImg() {
        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Read) != NativeGallery.Permission.Granted) {
            RequestPermission();
        }

        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Read) != NativeGallery.Permission.Granted)
            return;

        NativeGallery.GetImageFromGallery(OnGetImgPath);

    }

    private void OnGetImgPath(string path) {
        if (string.IsNullOrWhiteSpace(path))
            return;
        Texture2D tex = NativeGallery.LoadImageAtPath(path, markTextureNonReadable: false);

        _selectedLogoFromDevice = CreateSprite(tex);

        CallBacks.onLogoSelected?.Invoke();
    }


    private void RequestPermission() {
        NativeGallery.RequestPermission(NativeGallery.PermissionType.Read);
    }
    #endregion

    #region load logo
    public void LoadLogo() {
        _businessProfileManager.GetMyData(OnLoadLogo, OnErrorLoading, forceUpdate: true);
    }

    private void OnLoadLogo(BusinessProfileJsonData businessProfileJsonData) {
        if (businessProfileJsonData == null)
            return;

        string logoUrl = businessProfileJsonData.logo;
        if (!string.IsNullOrWhiteSpace(logoUrl)) {
            _webRequestHandler.GetTextureRequest(logoUrl,
                (code, body, texture) => {
                    UpdateLogo(CreateSprite((Texture2D)texture));
                },
                (code, body) => { TryLoadDefaultLogo(); }, nonreadable: false);
        } else {
            TryLoadDefaultLogo();
        }
    }

    private void TryLoadDefaultLogo() {
        string logoUrl = _authorizationAPIScriptableObject.DefaultLogoLink;
        if (!string.IsNullOrWhiteSpace(logoUrl)) {
            _webRequestHandler.GetTextureRequest(logoUrl,
                (code, body, texture) => {
                    UpdateLogo(CreateSprite((Texture2D)texture));
                },
                (code, body) => {
                    _logo = null;
                }, nonreadable: false);
        } else {
            _logo = null;
        }
    }

    private void OnErrorLoading(WebRequestError webRequestError) {
        HelperFunctions.DevLogError(webRequestError.Code + ": " + webRequestError.Detail);
    }

    #endregion

    #region upload logo
    private void OnUploadSelectedLogo() {
        Sprite currentSelected = _selectedLogoFromDevice;
        bool withTransparace = currentSelected.texture.format == TextureFormat.RGBA32 || currentSelected.texture.format == TextureFormat.ARGB32;

        HelperFunctions.DevLog("withTransparace: " + withTransparace + " format: " + currentSelected.texture.format);

        byte[] imageData = withTransparace ? ImageConversion.EncodeToPNG(currentSelected.texture) : ImageConversion.EncodeToJPG(currentSelected.texture);


        Dictionary<string, MultipartRequestBinaryData> formData = new Dictionary<string, MultipartRequestBinaryData>();
        MultipartRequestBinaryData multipartRequestBinaryData = new MultipartRequestBinaryData(
            "logo",
            imageData,
            "logo." + (withTransparace ? "png" : "jpg"));
        formData.Add("logo", multipartRequestBinaryData);

        string url = _webRequestHandler.ServerURLMediaAPI +
            _authorizationAPIScriptableObject.UpdateLogo.Replace("{id}", _businessProfileManager.GetID());

        HelperFunctions.DevLog(url);


        _webRequestHandler.PostMultipart(url,
            formData,
            (code, body) => {
                HelperFunctions.DevLog(code + ": " + body);
                OnUploaded(currentSelected); },
            (code, body) => { OnUploadedError(); }, needHeaderAccessToken: true);
    }

    private void OnUploaded(Sprite uploadedLogo) {
        UpdateLogo(uploadedLogo);
        CallBacks.onLogoUploaded?.Invoke();
    }

    private void OnUploadedError() {
        CallBacks.onLogoUploadingError?.Invoke();
    }
    #endregion

    #region remove
    private void OnRemove() {

    }
    #endregion

    private Sprite CreateSprite(Texture2D texture) {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    private void UpdateLogo(Sprite newLogo) {
        _logo = newLogo;
        CallBacks.onBusinessLogoUpdated?.Invoke();
    }

    ~BusinessLogoController() {
        CallBacks.onSelectLogoFromDevice -= SelectNewImg;
        CallBacks.onUploadSelectedLogo -= OnUploadSelectedLogo;
        CallBacks.onRemoveLogo -= OnRemove;
        CallBacks.onLoadLogo -= LoadLogo;
        CallBacks.onSignInSuccess -= LoadLogo;

        CallBacks.hasLogoOnDevice = null;
        CallBacks.getLogoOnDevice = null;
        CallBacks.getSelectedLogoOnDevice = null;
    }
}

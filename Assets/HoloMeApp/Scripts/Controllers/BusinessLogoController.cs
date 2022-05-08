using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

public class BusinessLogoController {

    private Sprite _logo;
    private Sprite _tempLogoFromDevice;

    public BusinessLogoController() {
        CallBacks.onUpdateLogoFromDevice += SelectNewImg;
        CallBacks.onUploadSelectedLogo += OnUploadSelectedLogo;

        CallBacks.hasLogoOnDevice = HasLogo;
        CallBacks.getLogoOnDevice = GetCurrentLogoImage;
        CallBacks.getSelectedLogoOnDevice = GetSelectedLogo;
    }

    private Sprite GetCurrentLogoImage() {
        return _logo;
    }

    private Sprite GetSelectedLogo() {
        return _tempLogoFromDevice;
    }

    private bool HasLogo() {
        return _logo != null;
    }

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
        Texture2D tex = NativeGallery.LoadImageAtPath(path);
        _tempLogoFromDevice = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        CallBacks.onLogoSelected?.Invoke();
    }


    private void RequestPermission() {
        NativeGallery.RequestPermission(NativeGallery.PermissionType.Read);
    }

    private void OnUploadSelectedLogo() {
        //TODO uploading
        OnUploaded();
    }

    private void OnUploaded() {
        CallBacks.onLogoUploaded?.Invoke();
    }

    private void OnUploadedError() {
        CallBacks.onLogoUploadingError?.Invoke();
    }

    ~BusinessLogoController() {
        CallBacks.hasLogoOnDevice = null;
        CallBacks.getLogoOnDevice = null;
        CallBacks.getSelectedLogoOnDevice = null;
        CallBacks.onUpdateLogoFromDevice -= SelectNewImg;
        CallBacks.onUploadSelectedLogo -= OnUploadSelectedLogo;
    }
}

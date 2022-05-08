using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using System.Threading.Tasks;
using System.Threading;

public class BusinessLogoController {

    private Sprite _logo;
    private Sprite _selectedLogoFromDevice;

    public BusinessLogoController() {
        CallBacks.onSelectLogoFromDevice += SelectNewImg;
        CallBacks.onUploadSelectedLogo += OnUploadSelectedLogo;
        CallBacks.onRemoveLogo += OnRemove;

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
        Texture2D tex = NativeGallery.LoadImageAtPath(path);
        _selectedLogoFromDevice = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        CallBacks.onLogoSelected?.Invoke();
    }


    private void RequestPermission() {
        NativeGallery.RequestPermission(NativeGallery.PermissionType.Read);
    }
    #endregion

    #region upload img
    private void OnUploadSelectedLogo() {
        //TODO add closure for selected

        Sprite currentSelected = _selectedLogoFromDevice;

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task delay = Task.Delay(1000);
        delay.ContinueWith(taskTokenID => {
            OnUploaded(currentSelected);
        }, taskScheduler);
    }

    private void OnUploaded(Sprite uploadedLogo) {
        _logo = uploadedLogo;
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

    ~BusinessLogoController() {
        CallBacks.onSelectLogoFromDevice -= SelectNewImg;
        CallBacks.onUploadSelectedLogo -= OnUploadSelectedLogo;
        CallBacks.onRemoveLogo -= OnRemove;

        CallBacks.hasLogoOnDevice = null;
        CallBacks.getLogoOnDevice = null;
        CallBacks.getSelectedLogoOnDevice = null;
    }
}

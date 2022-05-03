using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// CTA Link Window
/// </summary>
public class CTALinkOptionsWindow : MonoBehaviour, IBlindView {

    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    [SerializeField]
    private CustomInputField _ctaLabel;
    [SerializeField]
    private CustomInputField _ctaUrl;

    [SerializeField]
    private Button _updateDataBtn;

    [SerializeField]
    private GameObject _warning;

    [SerializeField]
    private TMP_Text _warningText;

    private string _warningTxt;
    private ARMsgJSON.Data _data;
    private WebRequestHandler _webRequestHandler;
    private PostARMsgExtDataController _postARMsgExtDataController;

    private const int DELAY_FOR_SUCCESS = 3000;
    private const string BUSINESS_OPTIONS_VIEW = "BusinessOptionsView";
    private const string SUCCESS_OPTIONS_VIEW = "SuccessOptionsView";
    private const string CTA_LINK_OPTIONS_VIEW = "CTALinkOptionsView";

    /// <summary>
    /// Show Window
    /// </summary>
    public void Show(params object[] objects) {

        if (objects != null && objects.Length > 0) {
            foreach (var item in objects) {
                Debug.Log(item.GetType());
                if (item is string) {
                    _warningTxt = item as string;
                } else if (item is ARMsgJSON.Data) {
                    _data = item as ARMsgJSON.Data;
                } else if (item is WebRequestHandler) {
                    _webRequestHandler = item as WebRequestHandler;
                }
            }
        }

        gameObject.SetActive(true);

        _warning.SetActive(!string.IsNullOrEmpty(_warningTxt));

        if (!string.IsNullOrEmpty(_warningTxt)) {
            _warningText.text = _warningTxt;
        }

        Debug.LogError(_webRequestHandler);

        if (_webRequestHandler != null) {
            _postARMsgExtDataController = new PostARMsgExtDataController(_arMsgAPIScriptableObject, _webRequestHandler);
        }

        if (_data != null && _data.ext_content_data != null) {
            _ctaLabel.UpdateText(_data.ext_content_data[0].cta_label);
            _ctaUrl.UpdateText(_data.ext_content_data[0].cta_url);
        }

        CheckText();

        _ctaLabel.GetInputField.onValueChanged.AddListener(OnValueChanged);
        _ctaUrl.GetInputField.onValueChanged.AddListener(OnValueChanged);
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
        _ctaLabel.GetInputField.onValueChanged.RemoveListener(OnValueChanged);
        _ctaUrl.GetInputField.onValueChanged.RemoveListener(OnValueChanged);
        gameObject.SetActive(false);
    }

    private void CheckText() {
        _updateDataBtn.interactable = _ctaUrl.Text.Length > 0 && _ctaLabel.Text.Length > 0;
    }

    private void OnValueChanged(string text) {
        CheckText();
    }

    private void EnableInput(bool status) {
        _ctaUrl.gameObject.SetActive(status);
        _ctaLabel.gameObject.SetActive(status);
    }

    /// <summary>
    /// Update Data Button
    /// </summary>
    public async void UpdateDataButton() {

        if (_data == null || _postARMsgExtDataController == null) {
            ShowError();
            return;
        }

        UnityWebRequest webRequest = UnityWebRequest.Get(_ctaUrl.Text);
        await webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success) {
            ShowError();
            return;
        }


        ARMsgJSON.Data.ExtContentData extContentData = new ARMsgJSON.Data.ExtContentData {
            cta_label = _ctaLabel.Text,
            cta_url = _ctaUrl.Text
        };
        Debug.LogError(extContentData.cta_label);
        Debug.LogError(extContentData.cta_url);
        _postARMsgExtDataController.PostARMsgExtDataById(_data.id, extContentData, ShowSuccess, (error) => ShowError());

    }

    private void ShowError() {
        EnableInput(false);
        WarningConstructor.ActivateDoubleButton(message: "Something went wrong", buttonOneText: "Retry", buttonTwoText: "Cancel", onButtonOnePress: UpdateDataButton, onButtonTwoPress: () => EnableInput(true), isWarning: true);
    }

    private async void ShowSuccess() {
        SuccessOptionsData data = new SuccessOptionsData(title: "Edit CTA", description: "The CTA information has\nbeen updated", backEvent: () => BlindOptionsConstructor.Show(CTA_LINK_OPTIONS_VIEW));
        BlindOptionsConstructor.Show(SUCCESS_OPTIONS_VIEW, data);

        await Task.Delay(DELAY_FOR_SUCCESS);

        BlindOptionsConstructor.Show(BUSINESS_OPTIONS_VIEW);
    }

}

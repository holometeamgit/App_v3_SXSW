using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Beem.SSO;
using System.Text.RegularExpressions;

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

    private object[] _lastCallobjects;

    private const string SUCCESS_OPTIONS_VIEW = "SubpnlCTALinkOptionsUpdatedWindow";
    private const string CTA_LINK_OPTIONS_VIEW = "CTALinkOptionsView";

    /// <summary>
    /// Show Window
    /// </summary>
    public void Show(params object[] objects) {
        _lastCallobjects = objects;
        if (objects != null && objects.Length > 0) {
            foreach (var item in objects) {
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

        ShowWarning(_warningTxt);

        if (_webRequestHandler != null) {
            _postARMsgExtDataController = new PostARMsgExtDataController(_arMsgAPIScriptableObject, _webRequestHandler);
        }

        if (_data != null && _data.ext_content_data != null) {
            if (_data.ext_content_data.Count == 0) {
                _ctaLabel.UpdateText("");
                _ctaUrl.UpdateText("");
            } else {
                _ctaLabel.UpdateText(_data.ext_content_data[0].cta_label);
                _ctaUrl.UpdateText(_data.ext_content_data[0].cta_url);
            }


        }

        CheckText();
        CustomInputField.OnShowKeyboard += OnInputField;
        _ctaLabel.GetInputField.onValueChanged.AddListener(OnValueChanged);
        _ctaUrl.GetInputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void ShowWarning(string text) {
    //    _warning.SetActive(!string.IsNullOrEmpty(text));

        _warningText.text = text;
    }

    private void OnInputField(bool isShown, int height) {
        if (isShown) {
            ShowWarning(string.Empty);
        } else {
            ShowWarning(_warningTxt);
        }
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
        CustomInputField.OnShowKeyboard -= OnInputField;
        _ctaLabel.GetInputField.onValueChanged.RemoveListener(OnValueChanged);
        _ctaUrl.GetInputField.onValueChanged.RemoveListener(OnValueChanged);
        gameObject.SetActive(false);
        _warningTxt = string.Empty;
    }

    private void CheckText() {
        _updateDataBtn.interactable = _ctaUrl.Text.Length > 0 && _ctaLabel.Text.Length > 0;
    }

    private void OnValueChanged(string text) {
        CheckText();
    }

    //TODO: move it all (UpdateDataButton,ShowError)  in controller
    /// <summary>
    /// Update Data Button
    /// </summary>
    public void UpdateDataButton() {

        if (_data == null || _postARMsgExtDataController == null) {
            ShowError("");
            return;
        }

        BlindOptionsConstructor.Show(SUCCESS_OPTIONS_VIEW);

        ARMsgJSON.Data.ExtContentData extContentData = new ARMsgJSON.Data.ExtContentData {
            cta_label = _ctaLabel.Text,
            cta_url = _ctaUrl.Text
        };

        _postARMsgExtDataController.PostARMsgExtDataById(_data.id, extContentData,
            () => {
                CallBacks.onUpdatedCTA?.Invoke(); _lastCallobjects = null;
                _data.ext_content_data[0] = extContentData; },
            (code, body)=> { ShowError(body); });
    }

    private void ShowError(string body) {

        string limit = Regex.Match(body, @"(?<=varying\()\d+").Value;

        if (body.Contains("value too long for type")) {
            WarningConstructor.ActivateSingleButton(header: " ",message: $"The link must not exceed {limit} characters",
                buttonText: "Confirm",
                onBackPress: () => BlindOptionsConstructor.Show(CTA_LINK_OPTIONS_VIEW, _lastCallobjects), isWarning: true);
        } else {
            WarningConstructor.ActivateDoubleButton(message: "Something went wrong",
                buttonOneText: "Retry", buttonTwoText: "Cancel",
                onButtonOnePress: UpdateDataButton, onButtonTwoPress: () => BlindOptionsConstructor.Show(CTA_LINK_OPTIONS_VIEW, _lastCallobjects), isWarning: true);
        }
    }
}

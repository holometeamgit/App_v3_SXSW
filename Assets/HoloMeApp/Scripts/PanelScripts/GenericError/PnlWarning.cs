using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Threading.Tasks;

public class PnlWarning : MonoBehaviour {
    [SerializeField]
    GameObject _titleGO;

    [SerializeField]
    GameObject _titleErrorGO;

    [SerializeField]
    TextMeshProUGUI txtHeader;

    [SerializeField]
    TextMeshProUGUI txtErrorHeader;

    [SerializeField]
    TextMeshProUGUI txtMessage;

    [SerializeField]
    Button btnLeft;

    [SerializeField]
    Button btnRight;

    const string DefaultHeader = "Error";

    const string DefaultMessage = "An error occurred please try again.";

    private void SetMessages(string header, string message, bool isWarning = false) {
        _titleGO.SetActive(!isWarning);
        _titleErrorGO.SetActive(isWarning);

        if (header == null || (string.IsNullOrWhiteSpace(header) && header != "")) {
            txtHeader.gameObject.SetActive(false);
            txtErrorHeader.gameObject.SetActive(false);
        } else {
            txtHeader.text = header == "" ? DefaultHeader : header;
            txtErrorHeader.text = header == "" ? DefaultHeader : header;
        }

        txtMessage.text = message == "" ? DefaultMessage : message;
    }

    void SetupButton(Button button, string text, UnityAction action) {
        button.gameObject.SetActive(true);
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        button.onClick.RemoveAllListeners(); //Remember this doesn't effect editor actions
        if (action != null) {
            button.onClick.AddListener(action);
        }
        button.onClick.AddListener(Deactivate);
    }

    public void ActivateSingleButton(string header = "", string message = "", string buttonText = "Back", UnityAction onBackPress = null, bool isWarning = false) {
        SetMessages(header, message, isWarning);
        SetupButton(btnLeft, buttonText, onBackPress);
        btnRight.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void ActivateDoubleButton(string header = "", string message = "", string buttonOneText = "Yes", string buttonTwoText = "No", UnityAction onButtonOnePress = null, UnityAction onButtonTwoPress = null, bool isWarning = false) {
        SetMessages(header, message, isWarning);
        SetupButton(btnLeft, buttonOneText, onButtonOnePress);
        SetupButton(btnRight, buttonTwoText, onButtonTwoPress);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        txtHeader.gameObject.SetActive(true);
        txtErrorHeader.gameObject.SetActive(true);
    }
}

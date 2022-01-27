using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Threading.Tasks;

public class PnlWarning : MonoBehaviour {
    [SerializeField]
    TextMeshProUGUI txtHeader;

    [SerializeField]
    TextMeshProUGUI txtMessage;

    [SerializeField]
    Button btnLeft;

    [SerializeField]
    Button btnRight;

    [SerializeField]
    Image imgWarning;

    const string DefaultHeader = "Error";

    const string DefaultMessage = "An error occurred please try again.";

    private void SetMessages(string header, string message) {
        if (header == null || (string.IsNullOrWhiteSpace(header) && header != "")) {
            txtHeader.gameObject.SetActive(false);
        } else {
            txtHeader.text = header == "" ? DefaultHeader : header;
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
        SetMessages(header, message);
        SetupButton(btnLeft, buttonText, onBackPress);
        btnRight.gameObject.SetActive(false);
        gameObject.SetActive(true);
        imgWarning.gameObject.SetActive(isWarning);
    }

    public void ActivateDoubleButton(string header = "", string message = "", string buttonOneText = "Yes", string buttonTwoText = "No", UnityAction onButtonOnePress = null, UnityAction onButtonTwoPress = null, bool isWarning = false) {
        SetMessages(header, message);
        SetupButton(btnLeft, buttonOneText, onButtonOnePress);
        SetupButton(btnRight, buttonTwoText, onButtonTwoPress);
        gameObject.SetActive(true);
        imgWarning.gameObject.SetActive(isWarning);
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        txtHeader.gameObject.SetActive(true);
        imgWarning.gameObject.SetActive(false);
    }
}

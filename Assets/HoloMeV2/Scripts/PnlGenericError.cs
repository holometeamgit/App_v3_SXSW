using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PnlGenericError : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtHeader;

    [SerializeField]
    TextMeshProUGUI txtMessage;

    [SerializeField]
    Button btnBack;

    const string DefaultHeader = "Error";

    const string DefaultMessage = "An error occurred please try again.";

    public void Activate(string header = "", string message = "", string buttonText = "Back", UnityAction onBackPress = null)
    {
        txtHeader.text = header == "" ? DefaultHeader : header;
        txtMessage.text = message == "" ? DefaultMessage : message;
        btnBack.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;

        btnBack.onClick.RemoveAllListeners();//Remember this doesn't effect editor actions
        if (onBackPress != null)
        {
            btnBack.onClick.AddListener(onBackPress);
        }

        gameObject.SetActive(true);
    }
}

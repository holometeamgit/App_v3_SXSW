using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Chat Message
/// </summary>
public class ChatMessage : MonoBehaviour {
    [SerializeField]
    private Text _userNameText;

    [SerializeField]
    private TMP_Text _tmpChatMessageText;


    /// <summary>
    /// Create Message
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="chatMessage"></param>
    public void TypeMessage(string userName, string chatMessage) {
        if (_userNameText != null) {
            _userNameText.text = userName;
        }

        if (_tmpChatMessageText != null) {
            _tmpChatMessageText.text = chatMessage;
        }
    }

    /// <summary>
    /// Delete Message
    /// </summary>
    public void DeleteMessage() {
        Destroy(this.gameObject);
    }
}

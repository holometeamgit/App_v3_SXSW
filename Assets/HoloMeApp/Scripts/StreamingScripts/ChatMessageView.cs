using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Chat Message View
/// </summary>
public class ChatMessageView : MonoBehaviour {
    [SerializeField]
    private TMP_Text _userNameText;

    [SerializeField]
    private TMP_Text _tmpChatMessageText;


    /// <summary>
    /// Create Message
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="chatMessage"></param>
    public void CreateMessage(string userName, string chatMessage) {
        gameObject.SetActive(true);

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
    public void DestroyMessage() {
        gameObject.SetActive(false);
    }
}

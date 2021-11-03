using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public void TypeMessage(string userName, string chatMessage) {
        if (_userNameText != null) {
            _userNameText.text = userName;
        }

        if (_tmpChatMessageText != null) {
            _tmpChatMessageText.text = chatMessage;
        }
    }
}

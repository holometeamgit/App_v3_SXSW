using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour {
    [SerializeField]
    private Text _userNameText;

    [SerializeField]
    private TMP_Text _chatMessageText;

    public void TypeMessage(string userName, string chatMessage) {
        if (_userNameText != null) {
            _userNameText.text = userName;
        }

        if (_chatMessageText != null) {
            _chatMessageText.text = chatMessage;
        }
    }

    public void DeleteMessage() {
        Destroy(this.gameObject);
    }
}

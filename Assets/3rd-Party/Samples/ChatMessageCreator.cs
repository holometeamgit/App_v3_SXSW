using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageCreator : MonoBehaviour {
    [SerializeField]
    private ChatMessage _prefabMessage;
    [SerializeField]
    private Transform _messagePlace;

    private string _userName = "Username";

    public void SetUserName(string userName) {
        _userName = userName;
    }

    public void TypeMessage(string chatMessage) {
        if (!string.IsNullOrEmpty(chatMessage)) {
            ChatMessage prefabMessage = Instantiate(_prefabMessage, _messagePlace);
            prefabMessage.TypeMessage(_userName, chatMessage);
        }

    }
}

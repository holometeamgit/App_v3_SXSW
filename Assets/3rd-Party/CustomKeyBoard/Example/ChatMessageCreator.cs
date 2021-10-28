using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Chat Message Creator
/// </summary>
public class ChatMessageCreator : MonoBehaviour {
    [SerializeField]
    private ChatMessage _prefabMessage;
    [SerializeField]
    private Transform _messagePlace;

    private string _userName = "Username";

    public void SetUserName(string userName) {
        _userName = userName;
    }

    /// <summary>
    /// Create Message
    /// </summary>
    /// <param name="chatMessage"></param>
    public void TypeMessage(string chatMessage) {
        if (!string.IsNullOrEmpty(chatMessage)) {
            ChatMessage prefabMessage = Instantiate(_prefabMessage, _messagePlace);
            prefabMessage.TypeMessage(_userName, chatMessage);
        }

    }
}

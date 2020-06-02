using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PnlStreamChat : MonoBehaviour
{
    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    GameObject chatMessagePrefabRef;

    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    RectTransform Content;

    [SerializeField]
    ScrollRect scrollRect;

    [SerializeField]
    VerticalLayoutGroup verticalLayoutGroup;

    List<GameObject> chatMessages = new List<GameObject>();

    //[SerializeField]
    //UnityEvent OnMessageRecieved; //For displaying notifications

    public void SendChatMessage(string message)
    {
        ChatMessageJsonData chatMessageJsonData = new ChatMessageJsonData { userName = "Test", message = message };
        CreateChatMessageGO(chatMessageJsonData);
        agoraController.SendMessage(JsonUtility.ToJson(chatMessageJsonData));

        inputField.text = "";
        StartCoroutine(RefreshLayoutGroup());

    }

    public void ReceivedChatMessage(ChatMessageJsonData chatMessageJsonData)
    {
        CreateChatMessageGO(chatMessageJsonData);
    }

    private void CreateChatMessageGO(ChatMessageJsonData chatMessageJsonData)
    {
        GameObject newMessageGO = Instantiate(chatMessagePrefabRef, Content, false);
        chatMessages.Add(newMessageGO);
        newMessageGO.transform.Find("txtUserName").GetComponent<TextMeshProUGUI>().text = chatMessageJsonData.userName;
        newMessageGO.transform.Find("txtMessage").GetComponent<TextMeshProUGUI>().text = chatMessageJsonData.message;

    }

    IEnumerator RefreshLayoutGroup()
    {
        verticalLayoutGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        verticalLayoutGroup.enabled = true;
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 1;
    }
}

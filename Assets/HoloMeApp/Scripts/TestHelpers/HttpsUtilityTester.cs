using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpsUtilityTester : MonoBehaviour {
    [SerializeField] string linkRoomId;

    [ContextMenu("SendRoomId")]
    public void SendRoomId() {
        HelperFunctions.DevLog(HttpUtility.UrlPathEncode(linkRoomId));
    }
}

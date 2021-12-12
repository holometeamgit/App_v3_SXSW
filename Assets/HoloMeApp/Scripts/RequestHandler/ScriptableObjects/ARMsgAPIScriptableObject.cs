using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ARMsgAPIScriptableObject settings
/// </summary>
[CreateAssetMenu(fileName = "ARMessageAPI", menuName = "Data/API/ARMessageAPI", order = 108)]
public class ARMsgAPIScriptableObject : ScriptableObject {
    [Header("AR Message")]
    public string ARMessageUpload = "/ar-message/upload/";
    public string ImgBGFieldName = "background_photo";
    public string SourceVideoFieldName = "source_video";
    [Space]
    public string UserARMessages = "/ar-message/";
    public string ARMessageById = "/ar-message/{id}";
    public string DeleteARMessageById = "/ar-message/{id}";
}
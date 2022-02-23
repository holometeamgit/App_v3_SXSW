using Beem.Firebase.CloudMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gallery Notification Controller
/// </summary>
public class GalleryNotificationController {

    private static List<ARMsgJSON.Data> _datas = new List<ARMsgJSON.Data>();

    public static bool IsNew(ARMsgJSON.Data data) {
        return _datas.Find(x => x.id == data.id && x.user == data.user && x.processing_status == ARMsgJSON.Data.COMPETED_STATUS) != null;
    }

    public void SetData(IDictionary<string, string> data) {
        ARMsgJSON.Data arMsgJsonData = new ARMsgJSON.Data();

        if (data.ContainsKey("id")) {
            arMsgJsonData.id = data["id"];
        }

        if (data.ContainsKey("share_link")) {
            arMsgJsonData.share_link = data["share_link"];
        }

        if (data.ContainsKey("ar_message_s3_link")) {
            arMsgJsonData.ar_message_s3_link = data["ar_message_s3_link"];
        }

        if (data.ContainsKey("processing_status")) {
            arMsgJsonData.processing_status = data["processing_status"];
        }

        if (data.ContainsKey("created_at")) {
            arMsgJsonData.created_at = data["created_at"];
        }

        if (data.ContainsKey("processed_at")) {
            arMsgJsonData.processed_at = data["processed_at"];
        }

        if (data.ContainsKey("user")) {
            arMsgJsonData.user = data["user"];
        }

        _datas.Add(arMsgJsonData);

        GalleryNotificationConstructor.OnShow?.Invoke();
    }

    /// <summary>
    /// Clear all Data
    /// </summary>
    public static void Clear() {
        _datas.Clear();
    }
}

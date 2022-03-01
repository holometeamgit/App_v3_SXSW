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

    public static Action OnShow = delegate { };
    public static Action OnHide = delegate { };

    private const string ID = "id";
    private const string USER = "user";

    public static bool IsNew(ARMsgJSON.Data data) {
        return _datas.Find(x => x.id == data.id && x.user == data.user && x.processing_status == ARMsgJSON.Data.COMPETED_STATUS) != null;
    }

    /// <summary>
    /// SetData
    /// </summary>
    /// <param name="data"></param>
    public void SetData(IDictionary<string, string> data) {
        ARMsgJSON.Data arMsgJsonData = new ARMsgJSON.Data();

        if (data.ContainsKey(ID)) {
            arMsgJsonData.id = data[ID];
        }

        if (data.ContainsKey(USER)) {
            arMsgJsonData.user = data[USER];
        }

        _datas.Add(arMsgJsonData);

        OnShow?.Invoke();
    }

    /// <summary>
    /// Clear all Data
    /// </summary>
    public static void Clear() {
        _datas.Clear();
    }
}

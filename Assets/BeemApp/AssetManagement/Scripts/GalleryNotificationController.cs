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

    public static Action<ARMsgJSON.Data> OnShow = delegate { };
    public static Action OnHide = delegate { };

    private GetARMsgController _getARMsgController;

    private const string ID = "id";

    public GalleryNotificationController(ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler) {
        _getARMsgController = new GetARMsgController(arMsgAPIScriptableObject, webRequestHandler);
    }

    /// <summary>
    /// Check on New Message
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool IsNew(ARMsgJSON.Data data) {
        return _datas.Find(x => x.id == data.id && x.user == data.user && data.processing_status == ARMsgJSON.Data.COMPETED_STATUS) != null;
    }

    /// <summary>
    /// Contains New Messages
    /// </summary>
    /// <returns></returns>
    public static bool ContainsNew() {
        return _datas.Count > 0;
    }

    /// <summary>
    /// SetData
    /// </summary>
    /// <param name="data"></param>
    public void SetData(IDictionary<string, string> data) {
        if (data.ContainsKey(ID)) {
            _getARMsgController.GetARMessage(id: data[ID], onSuccess: Add);
        }
    }

    private void Add(ARMsgJSON.Data data) {
        _datas.Add(data);
        OnShow?.Invoke(data);
    }

    /// <summary>
    /// Clear all Data
    /// </summary>
    public static void Clear() {
        _datas.Clear();
    }
}

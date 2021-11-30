using Beem.ARMsg;
using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//WIP
public class PnlDownloadedARMessages : MonoBehaviour {

    private List<IARMsgDataView> _arMsgDataViews;

    private void OnEnable() {
        //CallBacks.OnGetLastReadyARMsgData += GetLastReadyARMsgData;
    }

    private void OnDisable() {
        //CallBacks.OnGetLastReadyARMsgData -= GetLastReadyARMsgData;
    }

    //public ARMsgJSON.Data GetLastReadyARMsgData() {
    // return _lastLoadedARMsgJSON;
    //}

    private void Init(ARMsgJSON.Data data) {
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(data));
    }
}


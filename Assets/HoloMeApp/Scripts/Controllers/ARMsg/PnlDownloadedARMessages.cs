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
        Init(CallBacks.OnGetLastReadyARMsgData?.Invoke());
    }

    private void Init(ARMsgJSON.Data data) {
        Debug.LogError(data.share_link);
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(data));
    }
}


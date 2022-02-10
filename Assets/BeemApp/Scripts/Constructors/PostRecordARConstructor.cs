using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constructor for post AR REcord
/// </summary>
public class PostRecordARConstructor : MonoBehaviour {
    [SerializeField]
    private PnlPostRecord pnlPostRecord;

    public static Action<RecordARData> OnShow = delegate { };
    public static Action OnHide = delegate { };

    public static bool IsActive;

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show(RecordARData recordARData) {
        IsActive = true;
        pnlPostRecord.Show(recordARData);
    }

    private void Hide() {
        IsActive = false;
        pnlPostRecord.Hide();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gallery Constructor
/// </summary>
public class GalleryConstructor : MonoBehaviour {

    [SerializeField]
    private GalleryWindow _galleryView;

    public static Action<ARMsgJSON> OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void Start() {
        Test();
    }

    private void Test() {
        int number = 30;

        ARMsgJSON arMsgJSON = new ARMsgJSON();

        arMsgJSON.count = number;
        arMsgJSON.results = new List<ARMsgJSON.Data>();
        for (int i = 0; i < number; i++) {
            ARMsgJSON.Data data = new ARMsgJSON.Data();
            data.ar_message_s3_link = "https://s3.eu-west-2.amazonaws.com/dev.streams/00000010_BEEM_Jan_intro_holo_7113.m4v";
            data.processing_status = ARMsgJSON.Data.COMPETED_STATUS;
            arMsgJSON.results.Add(data);
        }

        Show(arMsgJSON);
    }

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show(ARMsgJSON data) {
        _galleryView.Show(data);
    }

    private void Hide() {
        _galleryView.Hide();
    }
}

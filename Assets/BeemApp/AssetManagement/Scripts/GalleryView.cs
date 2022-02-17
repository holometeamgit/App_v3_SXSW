using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GalleryView : MonoBehaviour {

    [SerializeField]
    private CellView cellView;
    [SerializeField]
    private Transform parent;

    private CancellationTokenSource cancelTokenSource;
    private const int DELAY = 100;

    private void Start() {

        int number = 25;

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

    public async void Show(ARMsgJSON arMsgJSON) {
        cancelTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancelTokenSource.Token;
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, (arMsgJSON.results.Count / 3 + 1) * 350);
        foreach (ARMsgJSON.Data item in arMsgJSON.results) {
            if (!cancellationToken.IsCancellationRequested) {
                CellView cell = Instantiate(cellView, parent);
                cell.Show(item);
                await Task.Yield();
            }
        }
    }

    private void OnDisable() {
        if (cancelTokenSource != null) {
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }
    }
}

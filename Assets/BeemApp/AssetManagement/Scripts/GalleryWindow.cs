using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Gallery View
/// </summary>
public class GalleryWindow : MonoBehaviour {

    [SerializeField]
    private CellView _cellView;
    [SerializeField]
    private RectTransform _scrollViewRect;
    [SerializeField]
    private Transform _parent;
    [SerializeField]
    private GameObject _empty;
    [SerializeField]
    private GameObject _notEmpty;

    private CancellationTokenSource cancelTokenSource;

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

    /// <summary>
    /// Show all elements
    /// </summary>
    /// <param name="arMsgJSON"></param>
    public async void Show(ARMsgJSON arMsgJSON) {
        gameObject.SetActive(true);
        if (arMsgJSON.count > 0) {
            _empty.SetActive(false);
            _notEmpty.SetActive(true);
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;

            int columnCount = 3;
            int rawCount = Mathf.CeilToInt(arMsgJSON.results.Count / columnCount);
            for (int i = 0; i < rawCount; i++) {
                for (int j = 0; j < columnCount; j++) {
                    CellView cell = Instantiate(_cellView, _parent);
                    RectTransform cellRect = cell.gameObject.GetComponent<RectTransform>();
                    cell.Show(arMsgJSON.results[columnCount * i + j]);
                    cellRect.localPosition -= Vector3.up * (i * cellRect.sizeDelta.y);
                    cellRect.localPosition += Vector3.right * (j * cellRect.sizeDelta.x);
                    await Task.Yield();
                }
            }
        } else {
            _empty.SetActive(true);
            _notEmpty.SetActive(false);
        }
    }

    private void UpdatePosition() {

    }

    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        if (cancelTokenSource != null) {
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }
    }
}

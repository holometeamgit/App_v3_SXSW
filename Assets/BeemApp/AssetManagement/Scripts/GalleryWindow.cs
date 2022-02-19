using DynamicScrollRect;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Gallery View
/// </summary>
public class GalleryWindow : MonoBehaviour {

    [SerializeField]
    private ScrollContent _content = null;
    [SerializeField]
    private GameObject _empty;
    [SerializeField]
    private GameObject _notEmpty;

    /// <summary>
    /// Show all elements
    /// </summary>
    /// <param name="arMsgJSON"></param>
    public void Show(ARMsgJSON arMsgJSON) {
        gameObject.SetActive(true);
        if (arMsgJSON.count > 0) {
            _empty.SetActive(false);
            _notEmpty.SetActive(true);

            List<ScrollItemData> contentDatas = new List<ScrollItemData>();

            for (int i = 0; i < arMsgJSON.count; i++) {
                ARMsgScrollItem aRMsgScrollItem = new ARMsgScrollItem(i);
                aRMsgScrollItem.Init(arMsgJSON.results[i]);
                contentDatas.Add(aRMsgScrollItem);
            }

            _content.InitScrollContent(contentDatas);
        } else {
            _empty.SetActive(true);
            _notEmpty.SetActive(false);
        }
    }

    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }
}

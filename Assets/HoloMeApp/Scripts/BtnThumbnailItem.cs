using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BtnThumbnailItem : MonoBehaviour
{
    [SerializeField]
    RawImage imgThumbnail;

    [SerializeField]
    GameObject imgPastLifeGO;

    [SerializeField]
    TMP_Text textPastLifeGO;

    [SerializeField]
    GameObject imgLifeGO;

    [SerializeField]
    TMP_Text txtPerformerName;

    [SerializeField]
    TMP_Text txtStateThumbnaul;

    [SerializeField]
    Button buttonComponent;
    string code;

    [SerializeField]
    AspectRatioFitterByMinSide aspectRatioFitterByMinSide;

    private string liveStageText = "Live show";
    private string pastLiveStageText = "Past broadcast";

    public void SetThumbnailPressAction(Action<string> OnPress)
    {
        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(() => OnPress?.Invoke(code));
    }

    public void UpdateThumbnailData(string code, Texture texture, string username = null)
    {
        imgThumbnail.texture = texture;
        this.code = code;
        if (txtPerformerName != null && username != null)
            txtPerformerName.text = username;
        aspectRatioFitterByMinSide?.Refresh();
    }

    //TODO in Beem v2 Add new type
    public void SetLiveState(bool value) {
        imgLifeGO.SetActive(value);
        imgPastLifeGO.SetActive(!value);

        if(txtStateThumbnaul != null)
            txtStateThumbnaul.text = value ? liveStageText : pastLiveStageText;
    }

    public void SetTimePeriod(DateTime streamDateTime) {
        if (textPastLifeGO == null)
            return;

        var timeSpan = DateTime.Now - streamDateTime;
        string outString = "Live ";

        if(timeSpan.Days > 365) {
            outString += timeSpan.Days % 365 + (timeSpan.Days % 365 == 1 ? " year" : " years") + " ago";
        } else if(timeSpan.Days > 1) {
            outString += timeSpan.Days + " days ago";
        } else if (timeSpan.Days == 1) {
            outString += timeSpan.Days + " day ago";
        } else if (timeSpan.Hours > 1) {
            outString += timeSpan.Hours + " hours ago";
        } else if (timeSpan.Hours == 1) {
            outString += timeSpan.Hours + " hour ago";
        } else if (timeSpan.Minutes > 1) {
            outString += timeSpan.Minutes + " minutes ago";
        } else if (timeSpan.Minutes == 1) {
            outString += timeSpan.Minutes + " minute ago";
        } else if (timeSpan.Seconds > 1) {
            outString += timeSpan.Seconds + " seconds ago";
        } else { 
            outString += 1 + " second ago";
        }

        textPastLifeGO.text = outString;
    }
}

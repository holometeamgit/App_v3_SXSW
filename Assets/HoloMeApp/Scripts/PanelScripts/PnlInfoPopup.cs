using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Info popup panel used for BeemMe, Room and Stadium
/// </summary>
public class PnlInfoPopup : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI txtTitle;

    [SerializeField]
    private TextMeshProUGUI txtMessage;

    [SerializeField]
    private GameObject greenScreenHintGameObject;

    [SerializeField]
    private GameObject[] InfoObjects;

    [SerializeField]
    private Image subMenuBackground;

    [SerializeField]
    private Sprite[] imgGradientBackgroundColours;

    [SerializeField]
    private Image imgGradientBackground;

    /// <summary>
    /// Show the info popup panel
    /// </summary>
    /// <param name="showGreenScreenHint">This will disable or enable the green screen hint line</param>
    public void Activate(string title, bool showGreenScreenHint, PnlInfoPopupColour backgroundColour) {
        ToggleHintIconLines(true);
        greenScreenHintGameObject.SetActive(showGreenScreenHint);
        AssignMessage(string.Empty);
        Init(title, backgroundColour);
    }


    /// <summary>
    /// Activate with sub message and no hint icons
    /// </summary>
    public void ActivateAsMessage(string title, string message, PnlInfoPopupColour backgroundColour) {
        ToggleHintIconLines(false);
        AssignMessage(message);
        Init(title, backgroundColour);
    }

    private void ToggleHintIconLines(bool enable) {
        foreach (GameObject gameObject in InfoObjects) {
            gameObject.SetActive(enable);
        }
    }

    private void Init(string title, PnlInfoPopupColour backgroundColour) {
        txtTitle.text = title;
        imgGradientBackground.sprite = imgGradientBackgroundColours[(int)backgroundColour];
        subMenuBackground.rectTransform.anchoredPosition = new Vector2(subMenuBackground.rectTransform.anchoredPosition.x, -Screen.height);
        subMenuBackground.rectTransform.DOAnchorPosY(0, .25f).SetEase(Ease.OutQuad);
        gameObject.SetActive(true);
    }

    private void AssignMessage(string message) {
        txtMessage.gameObject.SetActive(!string.IsNullOrEmpty(message));
        txtMessage.text = message;
    }

    /// <summary>
    /// Hide the popup
    /// </summary>
    public void Hide() {
        subMenuBackground.rectTransform.DOAnchorPosY(-Screen.height, .25f).OnComplete(() => gameObject.SetActive(false));
    }
}

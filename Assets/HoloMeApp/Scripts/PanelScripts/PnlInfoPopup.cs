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
    GameObject[] greenScreenHintGameObjects;

    [SerializeField]
    Image imgBackground;

    /// <summary>
    /// Show the info popup panel
    /// </summary>
    /// <param name="showGreenScreenHint">This will disable or enable the green screen hint line</param>
    public void Activate(string title, bool showGreenScreenHint, Color backgroundColour) {
        txtTitle.text = title;

        if (greenScreenHintGameObjects != null) {
            foreach (GameObject greenScreen in greenScreenHintGameObjects) {
                greenScreen.SetActive(showGreenScreenHint);
            }
        }

        imgBackground.color = backgroundColour;

        imgBackground.rectTransform.anchoredPosition = new Vector2(imgBackground.rectTransform.anchoredPosition.x, -Screen.height);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(imgBackground.rectTransform.DOAnchorPosY(Screen.height / 10, .5f));
        mySequence.Append(imgBackground.rectTransform.DOAnchorPosY(0, .5f).SetEase(Ease.OutQuad));

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide the popup
    /// </summary>
    public void Hide() {
        imgBackground.rectTransform.DOAnchorPosY(-Screen.height, .25f).OnComplete(() => gameObject.SetActive(false));
    }
}

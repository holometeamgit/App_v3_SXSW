using DG.Tweening;
using TMPro;
using UnityEngine;

public class StreamNotificationPopupWindow : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI txtCentreMessage;

    [SerializeField]
    private RectTransform CentreMessage;

    private string tweenAnimationID = nameof(tweenAnimationID);

    private void OnEnable() {
        txtCentreMessage.text = string.Empty;
        CentreMessage.localScale = Vector3.zero;
    }


    /// <summary>
    /// Show the message with a scale animation effect
    /// </summary>
    public void AnimatedCentreTextMessage(string message) {
        DOTween.Kill(tweenAnimationID);
        CentreMessage.localScale = Vector3.zero;
        txtCentreMessage.text = message;
        txtCentreMessage.color = new Color(txtCentreMessage.color.r, txtCentreMessage.color.g, txtCentreMessage.color.b, 1);
        CentreMessage.DOScale(Vector3.one, .1f).SetId(tweenAnimationID);
    }

    /// <summary>
    /// Hide the message
    /// </summary>
    public void AnimatedFadeOutMessage(float delay = 0) {
        txtCentreMessage.DOFade(0, .5f).SetDelay(delay).SetId(tweenAnimationID);
        CentreMessage.DOScale(Vector3.zero, .1f).SetDelay(delay).SetId(tweenAnimationID);
    }
}

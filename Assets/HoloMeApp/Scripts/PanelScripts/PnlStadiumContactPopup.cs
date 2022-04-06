using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PnlStadiumContactPopup : MonoBehaviour {

    [SerializeField]
    private Image subMenuBackground;

    public void Activate() {
        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
            subMenuBackground.rectTransform.anchoredPosition = new Vector2(subMenuBackground.rectTransform.anchoredPosition.x, -Screen.height);
            subMenuBackground.rectTransform.DOAnchorPosY(0, .25f).SetEase(Ease.OutQuad);
        }
    }

    public void Hide() {
        if (gameObject.activeInHierarchy) {
            subMenuBackground.rectTransform.DOAnchorPosY(-Screen.height, .25f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}

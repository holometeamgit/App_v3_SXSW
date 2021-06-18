using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class HoldButtonSimple : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField]
    Color holdColour;
    Color originalColour;

    public UnityEvent onTouchDown;
    public UnityEvent onTouchUp;

    [SerializeField]
    Image imageToAnimate;

    private void Awake() {
        if (imageToAnimate == null) {
            imageToAnimate = transform.GetComponent<Image>();
        }
        originalColour = imageToAnimate.color;
    }

    private void OnEnable() {
        imageToAnimate.transform.localScale = Vector3.one;
        imageToAnimate.color = originalColour;
    }

    public void OnPointerDown(PointerEventData eventData) {
        onTouchDown?.Invoke();
        imageToAnimate.transform.DOBlendableScaleBy(new Vector3(.25f, .25f, .25f), .1f);
        imageToAnimate.color = holdColour;
    }

    public void OnPointerUp(PointerEventData eventData) {
        onTouchUp?.Invoke();
        imageToAnimate.transform.DOBlendableScaleBy(-new Vector3(.25f, .25f, .25f), .1f);
        imageToAnimate.color = originalColour;
    }
}

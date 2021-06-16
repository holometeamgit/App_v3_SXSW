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

    Image image;
    private void Awake() {
        image = transform.GetComponent<Image>();
        originalColour = image.color;
    }

    private void OnEnable() {
        image.transform.localScale = Vector3.one;
        image.color = holdColour;
    }

    public void OnPointerDown(PointerEventData eventData) {
        onTouchDown?.Invoke();
        image.transform.DOBlendableScaleBy(Vector3.one, .1f);
        image.color = holdColour;
    }

    public void OnPointerUp(PointerEventData eventData) {
        onTouchUp?.Invoke();
        image.transform.DOBlendableScaleBy(-Vector3.one, .1f);
        image.color = originalColour;
    }
}

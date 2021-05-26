using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class HoldButtonSimple : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public UnityEvent onTouchDown;
    public UnityEvent onTouchUp;

    Image image;
    private void Awake() {
        image = transform.GetComponent<Image>();
    }

    private void OnEnable() {
        image.transform.localScale = Vector3.one;
        image.color = Color.green;
    }

    public void OnPointerDown(PointerEventData eventData) {
        onTouchDown?.Invoke();
        image.transform.DOBlendableScaleBy(Vector3.one, .25f);
        image.color = Color.red;
    }

    public void OnPointerUp(PointerEventData eventData) {
        onTouchUp?.Invoke();
        image.transform.DOBlendableScaleBy(-Vector3.one, .25f);
        image.color = Color.green;
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Use this class for press and hold buttons. It will auto scale button on hold and change the sprite colour to the holdColour
/// </summary>
[RequireComponent(typeof(Image))]
public class HoldButtonSimple : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField]
    Color holdColour;
    Color originalColour;
    Color notInteractableColour;
    private bool coloursAssigned;

    [SerializeField]
    private Vector3 scaleByOnPress = new Vector3(.5f, .5f, .5f);

    public UnityEvent onTouchDown;
    public UnityEvent onTouchUp;

    private bool interactable;
    private bool isHeld;

    /// <summary>
    /// This mimics Selectable functionality of Unity UI components
    /// </summary>
    public bool Interactable {
        get { return interactable; }
        set {
            interactable = value;
            if (!value && isHeld) {
                PointerUpSharedFunctions();
            }
            if (coloursAssigned) {
                imageToAnimate.color = value ? originalColour : notInteractableColour;
            }
        }
    }

    [SerializeField]
    private Image imageToAnimate;

    private void Awake() {
        if (imageToAnimate == null) {
            imageToAnimate = transform.GetComponent<Image>();
        }

        originalColour = imageToAnimate.color;
        notInteractableColour = HelperFunctions.GetColor(200, 200, 200, 128);
        coloursAssigned = true;
    }

    private void OnEnable() {
        imageToAnimate.transform.localScale = Vector3.one;
        imageToAnimate.color = originalColour;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!interactable)
            return;
        isHeld = true;
        onTouchDown?.Invoke();
        imageToAnimate.transform.DOBlendableScaleBy(scaleByOnPress, .1f);
        imageToAnimate.color = holdColour;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!interactable)
            return;
        isHeld = false;
        PointerUpSharedFunctions();
    }

    private void PointerUpSharedFunctions() {
        onTouchUp?.Invoke();
        imageToAnimate.transform.DOBlendableScaleBy(-scaleByOnPress, .1f);
        imageToAnimate.color = originalColour;
    }
}

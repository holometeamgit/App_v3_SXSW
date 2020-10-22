using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRefreshableScroll : ScrollRect {

    private bool dragging;
    public bool Dragging {
        get { return dragging; }
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);

        dragging = true;
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);

        dragging = false;
    }
}

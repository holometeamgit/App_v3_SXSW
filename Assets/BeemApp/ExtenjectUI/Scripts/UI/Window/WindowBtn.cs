using UnityEngine.EventSystems;

namespace Beem.Extenject.UI {
    /// <summary>
    /// Window Button
    /// </summary>
    public class WindowBtn : WindowCaller, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            CallWindow();
        }
    }
}

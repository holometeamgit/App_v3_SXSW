using UnityEngine.EventSystems;

namespace WindowManager.Extenject {
    /// <summary>
    /// Window Button
    /// </summary>
    public class WindowBtn : WindowCaller, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            CallWindow();
        }
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnityEvent OnHeldDown;
    public UnityEvent OnRelease;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnHeldDown?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnRelease?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnRelease?.Invoke();
    }
}

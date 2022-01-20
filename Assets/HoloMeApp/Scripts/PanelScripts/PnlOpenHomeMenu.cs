using UnityEngine;
using UnityEngine.Events;

public class PnlOpenHomeMenu : MonoBehaviour
{
    public UnityEvent OnEnabled;

    private void OnEnable()
    {
        OnEnabled?.Invoke();
    }
}

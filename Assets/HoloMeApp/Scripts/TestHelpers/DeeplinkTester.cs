using UnityEngine;

/// <summary>
/// Tester for DeepLinks
/// </summary>
public class DeeplinkTester : MonoBehaviour {
    [SerializeField]
    private string link;

    [ContextMenu("Call Link")]
    public void CallLink() {
        StreamCallBacks.onReceivedDeepLink?.Invoke(link);
    }
}

using UnityEngine;

public abstract class AgoraMessageReceiver : MonoBehaviour
{
    public abstract void ReceivedChatMessage(string data);
}
